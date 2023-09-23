{
  jdk17,
  maven,
  jdt-language-server,
  vscode,
  scenebuilder,
  stdenv,
  glib,
  xorg,
  lib,
  mkShell,
}:
mkShell {
  packages = [jdk17 maven jdt-language-server vscode scenebuilder];
  LD_LIBRARY_PATH = lib.makeLibraryPath [stdenv.cc.cc glib xorg.libXxf86vm xorg.libXtst];
}
