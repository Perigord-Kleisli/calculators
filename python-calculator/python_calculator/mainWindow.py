import gi
import copy
import math
import functools
from python_calculator.resources import *
from functools import *
from gi.repository import Gtk, Gdk, Gio
from enum import Enum


class Operation(Enum):
    ADD = 0
    SUB = 1
    MUL = 2
    DIV = 3
    NONE = 4


class Expr:
    def __init__(self, val, opp):
        self.val = val
        self.opp = opp

    def __str__(self):
        match self.opp:
            case Operation.ADD:
                return f"{self.val:g} + "
            case Operation.SUB:
                return f"{self.val:g} - "
            case Operation.MUL:
                return f"{self.val:g} × "
            case Operation.DIV:
                return f"{self.val:g} ÷ "
            case Operation.NONE:
                return str(self.val)


class MainWindow(Gtk.Window):
    def __init__(self):
        provider = Gtk.CssProvider()
        provider.load_from_data(style_css)
        Gtk.StyleContext.add_provider_for_screen(
            Gdk.Screen.get_default(), provider, 800
        )

        builder = Gtk.Builder()
        builder.add_from_string(main_glade)

        self.mainWindow = builder.get_object("MainWindow")
        self.mainWindow.connect("destroy", Gtk.main_quit)
        self.numpad0 = builder.get_object("numpad0")
        self.numpad1 = builder.get_object("numpad1")
        self.numpad2 = builder.get_object("numpad2")
        self.numpad3 = builder.get_object("numpad3")
        self.numpad4 = builder.get_object("numpad4")
        self.numpad5 = builder.get_object("numpad5")
        self.numpad6 = builder.get_object("numpad6")
        self.numpad7 = builder.get_object("numpad7")
        self.numpad8 = builder.get_object("numpad8")
        self.numpad9 = builder.get_object("numpad9")
        self.numpad_add = builder.get_object("numpad_add")
        self.numpad_sub = builder.get_object("numpad_sub")
        self.numpad_mul = builder.get_object("numpad_mul")
        self.numpad_div = builder.get_object("numpad_div")
        self.numpad_dot = builder.get_object("numpad_dot")
        self.numpad_eq = builder.get_object("numpad_eq")
        self.numpad_negate = builder.get_object("numpad_negate")
        self.numpad_sqrt = builder.get_object("numpad_sqrt")
        self.numpad_square = builder.get_object("numpad_square")
        self.numpad_cube = builder.get_object("numpad_cube")
        self.numpad_reciprocal = builder.get_object("numpad_reciprocal")
        self.numpad_percent = builder.get_object("numpad_percent")
        self.clearGlobal = builder.get_object("ClearGlobal")
        self.clearEntry = builder.get_object("ClearEntry")
        self.backspace = builder.get_object("Backspace")
        self.expression_label = builder.get_object("expression_label")
        self.input = builder.get_object("input")
        self.expression = []
        self.preview = False

        self.numpad0.connect("clicked", self.input_digit("0"))
        self.numpad1.connect("clicked", self.input_digit("1"))
        self.numpad2.connect("clicked", self.input_digit("2"))
        self.numpad3.connect("clicked", self.input_digit("3"))
        self.numpad4.connect("clicked", self.input_digit("4"))
        self.numpad5.connect("clicked", self.input_digit("5"))
        self.numpad6.connect("clicked", self.input_digit("6"))
        self.numpad7.connect("clicked", self.input_digit("7"))
        self.numpad8.connect("clicked", self.input_digit("8"))
        self.numpad9.connect("clicked", self.input_digit("9"))
        self.clearEntry.connect("clicked", self.clear_entry)
        self.clearGlobal.connect("clicked", self.clear_global)
        self.numpad_add.connect("clicked", self.update_expression(Operation.ADD))
        self.numpad_sub.connect("clicked", self.update_expression(Operation.SUB))
        self.numpad_mul.connect("clicked", self.update_expression(Operation.MUL))
        self.numpad_div.connect("clicked", self.update_expression(Operation.DIV))
        self.numpad_dot.connect("clicked", self.add_dot)
        self.numpad_eq.connect("clicked", self.eq)
        self.backspace.connect("clicked", self.backspace_fn)
        self.numpad_percent.connect("clicked", self.percent)
        self.numpad_negate.connect("clicked", self.negate)
        self.numpad_sqrt.connect("clicked", self.modify_input(math.sqrt))
        self.numpad_square.connect("clicked", self.modify_input(lambda x: x * x))
        self.numpad_cube.connect("clicked", self.modify_input(lambda x: x * x * x))
        self.numpad_reciprocal.connect("clicked", self.modify_input(lambda x: 1/x))

    def percent(self, button):
        try: 
            if len(self.expression) > 0:
                val = float(self.input.get_text())
                self.input.set_text(f'{(self.eval_expression() * (val/100)):g}')
        except ValueError:
            print("invalid input")

    def modify_input(self, fn):
        def f(button):
            try:
                val = float(self.input.get_text())
                self.input.set_text(f'{fn(val):g}')
            except ValueError:
                print("invalid input")
        return f

    def eq(self, button):
        txt = self.input.get_text()
        try: 
            val = float(txt)
            self.expression.append(Expr(val, Operation.NONE))
            self.input.set_text(f'{self.eval_expression():g}')
            self.expression.clear()
            self.expression_label.set_text("")
            self.preview = True
        except ValueError:
            print("invalid input")

    def backspace_fn(self, button):
        txt = self.input.get_text()
        if txt != "":
            self.input.set_text(txt[:-1])
            self.preview = False

    def add_dot(self, button):
        txt = self.input.get_text()
        if not "." in txt:
            self.input.set_text(txt + ".")

    def negate(self, button):
        txt = self.input.get_text()
        if txt.startswith("-"):
            self.input.set_text(txt[1:])
        elif len(txt) > 0:
            self.input.set_text("-" + txt)

    def eval_expression(self):
        def expression_operator(accum, x):
            match accum.opp:
                case Operation.ADD:
                    return Expr(accum.val + x.val, x.opp)
                case Operation.SUB:
                    return Expr(accum.val - x.val, x.opp)
                case Operation.MUL:
                    return Expr(accum.val * x.val, x.opp)
                case Operation.DIV:
                    return Expr(accum.val / x.val, x.opp)
                case Operation.NONE:
                    return Expr(accum.val, x.opp)

        return reduce(expression_operator, self.expression).val

    def update_expression(self, opp):
        def f(button):
            try:
                val = float(self.input.get_text())
                self.expression.append(Expr(val, opp))
                self.expression_label.set_text(
                    functools.reduce(
                        lambda accum, x: accum + str(x), self.expression, ""
                    )
                )

                if len(self.expression) > 1:
                    self.input.set_text(f"{self.eval_expression():g}")
                else:
                    self.input.set_text("")
                self.preview = True
            except:
                None

        return f

    def clear_entry(self, button):
        self.input.set_text("")
        self.preview = False

    def clear_global(self, button):
        self.input.set_text("")
        self.expression.clear()
        self.expression_label.set_text("")
        self.preview = False

    def input_digit(self, chr):
        def f(button):
            txt = self.input.get_text()
            if self.preview:
                self.input.set_text(str(chr))
                self.preview = False
            else:
                self.input.set_text(txt + str(chr))

        return f
