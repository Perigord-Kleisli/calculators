package com.perigord;

import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.function.UnaryOperator;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TextField;

public class Controller {

    @FXML
    public Button numpad_0;
    @FXML
    public Button numpad_1;
    @FXML
    public Button numpad_2;
    @FXML
    public Button numpad_3;
    @FXML
    public Button numpad_4;
    @FXML
    public Button numpad_5;
    @FXML
    public Button numpad_6;
    @FXML
    public Button numpad_7;
    @FXML
    public Button numpad_8;
    @FXML
    public Button numpad_9;
    @FXML
    public Button dot_button;
    @FXML
    public Button div_button;
    @FXML
    public Button add_button;
    @FXML
    public Button sub_button;
    @FXML
    public Button mul_button;
    @FXML
    public Button eq_button;
    @FXML
    public Button neg_button;
    @FXML
    public TextField input;
    @FXML
    public Button backspace;
    @FXML
    public Button clear;
    @FXML
    public Button clear_entry;
    @FXML
    public Button percent_button;
    @FXML
    public Button sqrt_button;
    @FXML
    public Button square_button;
    @FXML
    public Button cube_button;
    @FXML
    public Button recip_button;
    @FXML
    public Label expression_label;

    private boolean preview = false;

    private enum Operation {
        ADD, SUB, MUL, DIV, NONE
    }

    private record Expr(double val, Operation opp) {
    };

    private ArrayList<Expr> expression = new ArrayList<Expr>();

    private DecimalFormat format = new DecimalFormat("0.##");

    @FXML
    public void initialize() {
        numpad_0.setOnAction(add_digit('0'));
        numpad_1.setOnAction(add_digit('1'));
        numpad_2.setOnAction(add_digit('2'));
        numpad_3.setOnAction(add_digit('3'));
        numpad_4.setOnAction(add_digit('4'));
        numpad_5.setOnAction(add_digit('5'));
        numpad_6.setOnAction(add_digit('6'));
        numpad_7.setOnAction(add_digit('7'));
        numpad_8.setOnAction(add_digit('8'));
        numpad_9.setOnAction(add_digit('9'));
        dot_button.setOnAction(_event -> {
            String txt = input.getText();
            if (!txt.isEmpty() && !txt.contains("."))
                input.setText(txt + ".");
        });
        neg_button.setOnAction(_event -> {
            String txt = input.getText();
            if (txt.length() == 0)
                return;
            if (txt.charAt(0) == '-')
                input.setText(txt.substring(1));
            else
                input.setText("-" + txt);
        });
        backspace.setOnAction(_event -> {
            var txt = input.getText();
            if (!txt.isEmpty()) {
                input.setText(txt.substring(0, txt.length() - 1));
                preview = false;
            }
        });
        clear_entry.setOnAction(_event -> input.setText(""));
        clear.setOnAction(_event -> {
            expression.clear();
            expression_label.setText("");
            input.setText("");
        });
        add_button.setOnAction(update_expression(Operation.ADD));
        sub_button.setOnAction(update_expression(Operation.SUB));
        mul_button.setOnAction(update_expression(Operation.MUL));
        div_button.setOnAction(update_expression(Operation.DIV));
        eq_button.setOnAction(_event -> {
            try {
                double val = Double.parseDouble(input.getText());
                expression.add(new Expr(val, Operation.NONE));
                expression_label.setText(expression_string());
                input.setText(format.format(eval_expression()));
                expression.clear();
                preview = true;
            } catch (NumberFormatException e) {
            }
        });
        percent_button.setOnAction(modify_input(x -> {
            double val = Double.parseDouble(input.getText());
            return val * (x / 100);
        }));
        sqrt_button.setOnAction(modify_input(x -> Math.sqrt(x)));
        square_button.setOnAction(modify_input(x -> x * x));;
        cube_button.setOnAction(modify_input(x -> x * x * x));;
        recip_button.setOnAction(modify_input(x -> 1 / x));;
    }

    private EventHandler<ActionEvent> modify_input(UnaryOperator<Double> f) {
        return _event -> {
            try {
                double val = f.apply(Double.parseDouble(input.getText()));
                input.setText(format.format(val));
            } catch (NumberFormatException e) {
            }
        };
    }

    private double eval_expression() {
        return expression.stream().reduce((accum, x) -> switch (accum.opp()) {
            case ADD -> new Expr(accum.val() + x.val(), x.opp());
            case SUB -> new Expr(accum.val() - x.val(), x.opp());
            case MUL -> new Expr(accum.val() * x.val(), x.opp());
            case DIV -> new Expr(accum.val() / x.val(), x.opp());
            default -> throw new IllegalArgumentException();
        }).get().val();
    }

    private String expression_string() {
        return expression
                .stream()
                .map(x -> x.val() + switch (x.opp()) {
                    case ADD -> " + ";
                    case SUB -> " - ";
                    case MUL -> " × ";
                    case DIV -> " ÷ ";
                    default -> "";
                })
                .reduce("", (x, y) -> x + y);
    }


    private EventHandler<ActionEvent> update_expression(Operation opp) {
        return _event -> {
            try {
                double val = Double.parseDouble(input.getText());
                expression.add(new Expr(val, opp));
                expression_label.setText(expression_string());
                input.setText(format.format(eval_expression()));
                preview = true;
            } catch (NumberFormatException e) {
            }
        };
    }

    private EventHandler<ActionEvent> add_digit(char digit) {
        return event -> {
            String txt = input.getText();
            if (preview) {
                input.setText(Character.toString(digit));
                preview = false;
            } else {
                input.setText(txt + digit);
            }
        };
    }
}
