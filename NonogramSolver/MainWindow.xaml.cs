﻿using NonogramSolver.Solver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NonogramSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Nonogram _solvedNonogram;
        public MainWindow()
        {
            InitializeComponent();
            
            var createHandlers = true;
            MakeNonograms(createHandlers);
            this.KeyDown += (o, args) =>
            {
                if (args.Key == Key.Space)
                {
                    MakeNonograms(false);
                }
            };
        }

        private void MakeNonograms(bool createHandlers)
        {
            var rows = 10;
            var columns = 20;
            _solvedNonogram = NonogramFactory.MakeNonogram(rows, columns);


            DrawDelayed(() => _solvedNonogram, 0, true);

            DrawDelayed(() =>
            {
                var clone = _solvedNonogram.Clone() as Nonogram;
                clone.Clear();
                var solver = new Solver.Solver();
                solver.Solve(clone);
                return clone;
            }, 1, createHandlers);

            DrawDelayed(() =>
            {
                var clone = _solvedNonogram.Clone() as Nonogram;
                var solver = new Solver.Solver();
                solver.RecursivelySolve(clone);
                return clone;
            }, 2, createHandlers);


            DrawDelayed(() =>
            {
                var clone = _solvedNonogram.Clone() as Nonogram;
                var solver = new ArcConsistencySolver();
                solver.Solve(clone);
                return clone;
            }, 3, createHandlers);
        }

        

        private void DrawDelayed(Func<Nonogram> producer, int contentIndex, bool addHandler = false)
        {
            var task = Task.Run(producer)
                .ContinueWith(nonogram =>
                {
                    DrawNonogram(contentIndex, nonogram.Result, addHandler);
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void DrawNonogram(int contentIndex, Nonogram n, bool addHandler = false)
        {
            var scrollView = this.Content as ScrollViewer;
            var contentPanel = scrollView.Content as Panel;
            var stackPanel = contentPanel.Children[contentIndex] as Panel;
            var nonogramView = stackPanel.Children[0] as NonogramView;
            var textBlock = stackPanel.Children[stackPanel.Children.Count - 1] as TextBlock;
            nonogramView.Nonogram = n;
            if (addHandler)
            {
                nonogramView.PropertyChanged += (sender, args) =>
                {
                    var view = sender as NonogramView;
                    UpdateTextBox(view.Nonogram, textBlock);
                };
                UpdateTextBox(n, textBlock);
            }
        }

        private void UpdateTextBox(Nonogram n, TextBlock blk)
        {

            var solver = new Solver.Solver();
            for (int i = 0; i < n.Height; i++)
            {
                for (int j = 0; j < n.Width; j++)
                {
                    var expected = _solvedNonogram.Cells[i][j].State;
                    var real = n.Cells[i][j].State;
                    if (solver.GetRowStatus(n.getRow(i), n.RowDescriptors[i]) == RowStatus.ContainsErrors)
                    {
                        blk.Text = $"found error at: row {i}";
                        return;
                    }

                    if (solver.GetRowStatus(n.getColumn(j), n.ColumnDescriptors[j]) == RowStatus.ContainsErrors)
                    {
                        blk.Text = $"found error at: col {j}";
                        return;
                    }
                }
            }
            blk.Text = "no errors hooray";
        }
        
    }
}
