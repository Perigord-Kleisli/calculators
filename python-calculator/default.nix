{
  poetry2nix,
  buildPackages,
  ninja,
  cairo,
  lib,
  gtk3,
  gobject-introspection, pkg-config,
}:
poetry2nix.mkPoetryApplication {
  projectDir = ./.;
  overrides = poetry2nix.defaultPoetryOverrides.extend (self: super: {
    pycairo = super.pycairo.overridePythonAttrs (
      _: {
        nativeBuildInputs = [
          self.meson
          buildPackages.ninja
          buildPackages.pkg-config
          ninja
        ];
        propogatedBuildInputs = [cairo];
      }
    );

    dontWrapGApps = true;
    nativeBuildInputs = [gobject-introspection pkg-config];
    buildInputs = [gtk3];
    preFixup = ''
      # Let python wrapper use GNOME flags.
      makeWrapperArgs+=(
        # For broadway daemons
        --prefix PATH : "${lib.makeBinPath [gtk3]}"
        "''${gappsWrapperArgs[@]}"
      )
    '';

    pygobject = super.pygobject.overridePythonAttrs (
      old: {
        buildInputs = (old.buildInputs or []) ++ [super.setuptools];
        propagatedBuildInputs = [cairo self.pycairo];
      }
    );
  });
}
