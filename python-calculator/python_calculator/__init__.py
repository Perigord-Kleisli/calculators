import gi


gi.require_version("Gtk", "3.0")
from gi.repository import Gtk, Gdk, Gio

import python_calculator.mainWindow

def main():
    win = mainWindow.MainWindow().mainWindow
    win.show_all()
    Gtk.main()

if __name__ == "__main__":
    main()
