using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Evaluator.Core;

namespace Evaluator.UI.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // --- Botones numéricos y operadores (incluye ^, (, ), ., +, -, *, /) ---
        public void OnKeyClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                var display = this.FindControl<TextBox>("Display");
                if (display is null) return;

                if (sender is Button btn)
                {
                    var key = btn.Content?.ToString();
                    if (!string.IsNullOrEmpty(key))
                    {
                        display.Text = (display.Text ?? string.Empty) + key;
                    }
                }
            }
            catch { /* evita cierre de la app si algo falla */ }
        }

        // --- Delete: borrar último carácter ---
        public void OnDelete(object? sender, RoutedEventArgs e)
        {
            try
            {
                var display = this.FindControl<TextBox>("Display");
                if (display is null) return;

                var text = display.Text ?? string.Empty;
                if (text.Length > 0)
                {
                    display.Text = text[..^1];
                }
            }
            catch { }
        }

        // --- Clear: limpiar todo ---
        public void OnClear(object? sender, RoutedEventArgs e)
        {
            try
            {
                var display = this.FindControl<TextBox>("Display");
                if (display is null) return;
                display.Text = string.Empty;
            }
            catch { }
        }

        // --- Evaluar expresión ---
        public void OnEquals(object? sender, RoutedEventArgs e)
        {
            try
            {
                var display = this.FindControl<TextBox>("Display");
                if (display is null) return;

                var expr = (display.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(expr)) return;

                var result = ExpressionEvaluator.Evaluate(expr);
                display.Text = result.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                var display = this.FindControl<TextBox>("Display");
                if (display is not null)
                    display.Text = "Error: " + ex.Message;
            }
        }
    }
}
