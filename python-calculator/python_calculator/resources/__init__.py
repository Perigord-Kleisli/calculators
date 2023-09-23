from importlib_resources import files

style_css = files('python_calculator.resources').joinpath("style.css").read_text()
main_glade = files('python_calculator.resources').joinpath("Main.glade").read_text()
