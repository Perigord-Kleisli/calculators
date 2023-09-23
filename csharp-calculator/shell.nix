{
  mkShell,
  dotnet-sdk,
  omnisharp-roslyn,
  clang-tools,
  glade,
  nodePackages_latest,
  stdenv,
  lib,
  gtk3,
}:
mkShell {
  packages = [dotnet-sdk omnisharp-roslyn clang-tools glade nodePackages_latest.prettier];
  LD_LIBRARY_PATH = lib.makeLibraryPath [stdenv.cc.cc gtk3];
}
