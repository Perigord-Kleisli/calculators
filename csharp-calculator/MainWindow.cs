using Gtk;
using System.Linq;
using System.Collections.Generic;
using UI = Gtk.Builder.ObjectAttribute;
using System;

namespace csharp
{
    class MainWindow : Window
    {

        [UI] private Button numpad0 = null;
        [UI] private Button numpad1 = null;
        [UI] private Button numpad2 = null;
        [UI] private Button numpad3 = null;
        [UI] private Button numpad4 = null;
        [UI] private Button numpad5 = null;
        [UI] private Button numpad6 = null;
        [UI] private Button numpad7 = null;
        [UI] private Button numpad8 = null;
        [UI] private Button numpad9 = null;
        [UI] private Button numpad_add = null;
        [UI] private Button numpad_sub = null;
        [UI] private Button numpad_mul = null;
        [UI] private Button numpad_div = null;
        [UI] private Button numpad_dot = null;
        [UI] private Button numpad_eq = null;
        [UI] private Button numpad_negate = null;
        [UI] private Button numpad_sqrt = null;
        [UI] private Button numpad_square = null;
        [UI] private Button numpad_cube = null;
        [UI] private Button numpad_reciprocal = null;
        [UI] private Button numpad_percent = null;
        [UI] private Button ClearGlobal = null;
        [UI] private Button ClearEntry = null;
        [UI] private Button Backspace = null;
        [UI] private Label expression_label = null;
        [UI] private Entry input = null;

        public MainWindow() : this(new Builder("Main.glade")) { }


        private enum Operation
        {
            ADD, SUB, MUL, DIV, NONE
        }

        private string stringFromOperation(Operation opp)
        {
            switch (opp)
            {
                case Operation.ADD: return " + ";
                case Operation.SUB: return " - ";
                case Operation.MUL: return " × ";
                case Operation.DIV: return " ÷ ";
                default: return "";
            }
        }

        private class Expr
        {
            public double value;
            public Operation operation;
        }

        List<Expr> expression = new List<Expr>();
        bool preview = false;

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            DeleteEvent += Window_DeleteEvent;
            builder.Autoconnect(this);

            this.KeyPressEvent += (e, a) =>
            {
                switch (a.Event.Key)
                {
                    case Gdk.Key.Key_0: input.GrabFocus(); break;
                    case Gdk.Key.Key_1: input.GrabFocus(); break;
                    case Gdk.Key.Key_2: input.GrabFocus(); break;
                    case Gdk.Key.Key_3: input.GrabFocus(); break;
                    case Gdk.Key.Key_4: input.GrabFocus(); break;
                    case Gdk.Key.Key_5: input.GrabFocus(); break;
                    case Gdk.Key.Key_6: input.GrabFocus(); break;
                    case Gdk.Key.Key_7: input.GrabFocus(); break;
                    case Gdk.Key.Key_8: input.GrabFocus(); break;
                    case Gdk.Key.Key_9: input.GrabFocus(); break;
                    case Gdk.Key.plus: numpad_add.Activate(); break;
                    case Gdk.Key.minus: numpad_sub.Activate(); break;
                    case Gdk.Key.asterisk: numpad_mul.Activate(); break;
                    case Gdk.Key.slash: numpad_div.Activate(); break;
                    case Gdk.Key.BackSpace: Backspace.Activate(); break;
                    case Gdk.Key.KP_Enter: numpad_eq.Activate(); break;
                    case Gdk.Key.equal: numpad_eq.Activate(); break;
                }
            };

            input.Changed += (object o, EventArgs e) =>
            {
                if (expression_label.Text == "Invalid Number")
                {
                    expression_label.Text = expression
                      .Select(e => e.value.ToString() + stringFromOperation(e.operation))
                      .Aggregate("", (x, accum) => x + accum);
                }
                else if (input.Text.Length > 0)
                {
                    switch (input.Text.Last())
                    {
                        case '+': numpad_add.Activate(); break;
                        case '-': numpad_sub.Activate(); break;
                        case '/': numpad_div.Activate(); break;
                        case '*': numpad_mul.Activate(); break;
                        case '=': numpad_eq.Activate(); break;
                        default: 
                          double val = 0;
                          if (!double.TryParse(input.Text, out val))
                            expression_label.Text = "Invalid Number";
                          return;
                    }
                    input.Text = input.Text.Remove(input.Text.Length - 1);
                }

            };

            numpad0.Clicked += input_digit('0');
            numpad1.Clicked += input_digit('1');
            numpad2.Clicked += input_digit('2');
            numpad3.Clicked += input_digit('3');
            numpad4.Clicked += input_digit('4');
            numpad5.Clicked += input_digit('5');
            numpad6.Clicked += input_digit('6');
            numpad7.Clicked += input_digit('7');
            numpad8.Clicked += input_digit('8');
            numpad9.Clicked += input_digit('9');
            numpad_negate.Clicked += delegate
            {
                if (input.Text.Length == 0) return;
                if (input.Text[0] == '-')
                    input.Text = input.Text.Remove(0, 1);
                else
                    input.Text = '-' + input.Text;

            };
            numpad_dot.Clicked += delegate
            {
                if (!input.Text.Any(x => x == '.'))
                    input.Text += ".";
            };
            numpad_add.Clicked += update_expression(Operation.ADD);
            numpad_sub.Clicked += update_expression(Operation.SUB);
            numpad_mul.Clicked += update_expression(Operation.MUL);
            numpad_div.Clicked += update_expression(Operation.DIV);
            numpad_eq.Clicked += delegate
            {
                if (expression.Count != 0)
                {
                    double val = 0;
                    if (!double.TryParse(input.Text, out val))
                    {
                        if (expression.Last().operation == Operation.MUL
                            || expression.Last().operation == Operation.DIV)
                            val = 1;
                    }
                    expression.Add(new Expr { value = val, operation = Operation.NONE });

                    expression_label.Text = expression
                      .Select(e => e.value.ToString() + stringFromOperation(e.operation))
                      .Aggregate("", (x, accum) => x + accum);
                    input.Text = evaluate_expression().ToString();
                    preview = true;
                    expression = new List<Expr>();
                }
            };

            ClearEntry.Clicked += delegate { input.Text = ""; };
            ClearGlobal.Clicked += delegate
            {
                input.Text = "";
                expression_label.Text = "";
                expression = new List<Expr>();
            };

            Backspace.Clicked += delegate
            {
                if (!preview && input.Text.Length > 0)
                    input.Text = input.Text.Remove(input.Text.Length - 1);
            };

            numpad_sqrt.Clicked += delegate
            {
                double val = 0;
                if (double.TryParse(input.Text, out val))
                {
                    input.Text = (Math.Sqrt(val)).ToString();
                }
            };
            numpad_square.Clicked += delegate
            {
                double val = 0;
                if (double.TryParse(input.Text, out val))
                {
                    input.Text = (Math.Pow(val, 2)).ToString();
                }
            };
            numpad_cube.Clicked += delegate
            {
                double val = 0;
                if (double.TryParse(input.Text, out val))
                {
                    input.Text = (Math.Pow(val, 3)).ToString();
                }
            };
            numpad_reciprocal.Clicked += delegate
            {
                double val = 0;
                if (double.TryParse(input.Text, out val))
                {
                    input.Text = (1 / val).ToString();
                }
            };
            numpad_percent.Clicked += delegate
            {
                double val = 0;
                if (double.TryParse(input.Text, out val))
                {
                    double total = evaluate_expression();
                    input.Text = (total * val / 100).ToString();
                }
            };
        }

        private double evaluate_expression()
        {
            return expression.Aggregate((accum, x) => accum.operation switch
            {
                Operation.ADD => new Expr { value = accum.value + x.value, operation = x.operation },
                Operation.SUB => new Expr { value = accum.value - x.value, operation = x.operation },
                Operation.MUL => new Expr { value = accum.value * x.value, operation = x.operation },
                Operation.DIV => new Expr { value = accum.value / x.value, operation = x.operation },
                _ => throw new NotImplementedException()
            }).value;
        }

        private EventHandler input_digit(char digit)
        {
            return delegate
            {
                if (preview)
                {
                    input.Text = digit.ToString();
                    preview = false;
                }
                else
                    input.Text += digit;
            };
        }

        private EventHandler update_expression(Operation opp)
        {
            return delegate
            {
                double val = 0;
                if (double.TryParse(input.Text, out val))
                    expression.Add(new Expr { value = val, operation = opp });

                if (expression.Count >= 2)
                {
                    input.Text = evaluate_expression().ToString();
                    preview = true;
                }
                else
                    input.Text = "";

                expression_label.Text = expression
                  .Select(e => e.value.ToString() + stringFromOperation(e.operation))
                  .Aggregate("", (x, accum) => x + accum);
            };
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

    }
}
