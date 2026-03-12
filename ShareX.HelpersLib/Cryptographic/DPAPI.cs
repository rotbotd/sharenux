#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace ShareX.HelpersLib
{
    // Cross-platform replacement for Windows DPAPI
    // Uses AES-256-GCM with a machine-specific key derived from user/machine identifiers
    public static class DPAPI
    {
        public enum DataProtectionScope
        {
            CurrentUser = 0,
            LocalMachine = 1
        }

        private static byte[] GetMachineKey(DataProtectionScope scope)
        {
            // Build a machine-specific seed from available identifiers
            string seed;
            
            if (scope == DataProtectionScope.CurrentUser)
            {
                // User-specific: combine username, home directory, and machine id
                seed = Environment.UserName + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            else
            {
                // Machine-wide: use machine identifiers only
                seed = Environment.MachineName;
            }

            // On Linux, add machine-id if available (stable across reboots)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string machineIdPath = "/etc/machine-id";
                if (File.Exists(machineIdPath))
                {
                    seed += File.ReadAllText(machineIdPath).Trim();
                }
            }
            // On macOS, use hardware UUID
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // IOPlatformUUID would require native calls, fall back to hostname
                seed += Environment.MachineName;
            }

            // Derive a 256-bit key using PBKDF2
            using var pbkdf2 = new Rfc2898DeriveBytes(
                Encoding.UTF8.GetBytes(seed),
                Encoding.UTF8.GetBytes("ShareNux.DPAPI.Salt"),
                100000,
                HashAlgorithmName.SHA256);
            
            return pbkdf2.GetBytes(32);
        }

        public static string Encrypt(string stringToEncrypt, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] data = Encoding.UTF8.GetBytes(stringToEncrypt);
            return Encrypt(data, optionalEntropy, scope);
        }

        public static string Encrypt(byte[] data, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] entropyData = null;
            if (optionalEntropy != null)
            {
                entropyData = Encoding.UTF8.GetBytes(optionalEntropy);
            }
            return Encrypt(data, entropyData, scope);
        }

        public static string Encrypt(byte[] data, byte[] optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] key = GetMachineKey(scope);
            
            // Mix in optional entropy if provided
            if (optionalEntropy != null && optionalEntropy.Length > 0)
            {
                using var hmac = new HMACSHA256(key);
                key = hmac.ComputeHash(optionalEntropy);
            }

            // Generate random nonce (12 bytes for AES-GCM)
            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            // Encrypt with AES-GCM
            byte[] ciphertext = new byte[data.Length];
            byte[] tag = new byte[16];

            using (var aes = new AesGcm(key, 16))
            {
                aes.Encrypt(nonce, data, ciphertext, tag);
            }

            // Output format: nonce (12) + tag (16) + ciphertext
            byte[] result = new byte[12 + 16 + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, 12);
            Buffer.BlockCopy(tag, 0, result, 12, 16);
            Buffer.BlockCopy(ciphertext, 0, result, 28, ciphertext.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string encryptedString, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedString);
            return Decrypt(encryptedData, optionalEntropy, scope);
        }

        public static string Decrypt(byte[] encryptedData, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] entropyData = null;
            if (optionalEntropy != null)
            {
                entropyData = Encoding.UTF8.GetBytes(optionalEntropy);
            }
            return Decrypt(encryptedData, entropyData, scope);
        }

        public static string Decrypt(byte[] encryptedData, byte[] optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (encryptedData.Length < 28)
            {
                throw new CryptographicException("Invalid encrypted data format");
            }

            byte[] key = GetMachineKey(scope);
            
            // Mix in optional entropy if provided
            if (optionalEntropy != null && optionalEntropy.Length > 0)
            {
                using var hmac = new HMACSHA256(key);
                key = hmac.ComputeHash(optionalEntropy);
            }

            // Parse: nonce (12) + tag (16) + ciphertext
            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] ciphertext = new byte[encryptedData.Length - 28];

            Buffer.BlockCopy(encryptedData, 0, nonce, 0, 12);
            Buffer.BlockCopy(encryptedData, 12, tag, 0, 16);
            Buffer.BlockCopy(encryptedData, 28, ciphertext, 0, ciphertext.Length);

            // Decrypt with AES-GCM
            byte[] plaintext = new byte[ciphertext.Length];

            using (var aes = new AesGcm(key, 16))
            {
                aes.Decrypt(nonce, ciphertext, tag, plaintext);
            }

            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
