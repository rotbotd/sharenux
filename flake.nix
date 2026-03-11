{
  description = "ShareNux - Cross-platform ShareX fork";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = nixpkgs.legacyPackages.${system};
      in
      {
        devShells.default = pkgs.mkShell {
          buildInputs = with pkgs; [
            # .NET 9
            dotnet-sdk_9

            # Avalonia dependencies
            libGL
            xorg.libX11
            xorg.libXcursor
            xorg.libXrandr
            xorg.libXi
            xorg.libXinerama
            xorg.libICE
            xorg.libSM
            fontconfig
            libxkbcommon

            # SkiaSharp dependencies
            libglvnd
            freetype
            harfbuzz

            # Screen capture (Linux)
            pipewire
            libportal
            
            # FFmpeg for screen recording
            ffmpeg

            # OCR
            tesseract

            # Development tools
            omnisharp-roslyn
            netcoredbg
          ];

          shellHook = ''
            export DOTNET_ROOT="${pkgs.dotnet-sdk_9}"
            export LD_LIBRARY_PATH="${pkgs.lib.makeLibraryPath [
              pkgs.libGL
              pkgs.xorg.libX11
              pkgs.xorg.libXcursor
              pkgs.xorg.libXrandr
              pkgs.xorg.libXi
              pkgs.xorg.libXinerama
              pkgs.xorg.libICE
              pkgs.xorg.libSM
              pkgs.fontconfig
              pkgs.libxkbcommon
              pkgs.libglvnd
              pkgs.freetype
              pkgs.harfbuzz
            ]}:$LD_LIBRARY_PATH"
            
            echo "ShareNux dev environment loaded"
            echo "dotnet version: $(dotnet --version)"
          '';
        };
      }
    );
}
