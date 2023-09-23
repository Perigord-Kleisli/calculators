{
  mkShell,
  poetry,
  python311,
  python311Packages,
  gobject-introspection,
  gtk3,
  glade,
}:
mkShell {
  packages = [
    poetry
    python311
    python311Packages.jedi-language-server
    python311Packages.pylint
    python311Packages.black
    gobject-introspection
    gtk3
    glade
  ];
}
