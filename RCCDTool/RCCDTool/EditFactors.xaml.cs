﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Controls;

namespace RCCDTool
{
    /// <summary>
    /// Interaction logic for EditFactors.xaml
    /// </summary>
    partial class EditFactorsViewer : Window
    {
        private int _numFactors;
        private IController _controller;
        private List<List<string>> listOfAllLabels;

        public EditFactorsViewer(int numFactors, IController controller)
        {
            
            InitializeComponent();
            _controller = controller;
            
            _numFactors = numFactors;

            if (_controller.ModelHasData)
            {
                _numFactors = _controller.NumFactors < _numFactors ? numFactors : _controller.NumFactors;

            }
            listOfAllLabels = new List<List<string>>();
            for (int i = 0; i < _numFactors; i++)
                listOfAllLabels.Add(new List<string>());
            

            CreateGrid(_numFactors); 
            buildTable(_numFactors);
    
        }

      
        private void CreateGrid(int numFactors)
        {
            factorGrid.ShowGridLines = false;
            this.MinWidth = 850;
            this.MinHeight = 125*numFactors;
            //factorGrid.MinHeight = this.MinHeight;
            
            //create columns            
            ColumnDefinition col1 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }; 
            ColumnDefinition col2 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
            ColumnDefinition col3 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
            ColumnDefinition col4 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
            //ColumnDefinition col5 = new ColumnDefinition();

            factorGrid.ColumnDefinitions.Add(col1);
            factorGrid.ColumnDefinitions.Add(col2);
            factorGrid.ColumnDefinitions.Add(col3);
            factorGrid.ColumnDefinitions.Add(col4);
            //factorGrid.ColumnDefinitions.Add(col5);

            //create rows based on number of factors
            for (int i = 0; i <= numFactors; i++)
                factorGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto } );

            //Create Column Headers
            TextBlock col = new TextBlock();
            setHeaderProperties(col, "Factor Name:", 0, 0);

            col = new TextBlock();
            setHeaderProperties(col, "Factor Levels:", 0, 1);

            col = new TextBlock();
            setHeaderProperties(col, "Randomize factor:", 0, 2);

            col = new TextBlock();
            setHeaderProperties(col, "Within/Between Subjects:", 0, 3);

            //col = new TextBlock();
            //setHeaderProperties(col, "Factor Labels:", 0, 4);

        }

        public void buildTable(int numFactors)
        {
            for (int i = 0; i < numFactors; i++)
            {
                //box for label
                var tb = new TextBox
                {
                    Height = 20,
                    Width = 170,
                    Name = "factorName"
                };


                Grid.SetRow(tb, i + 1);
                Grid.SetColumn(tb, 0);
                

                //box for number of levels
                var tb2 = new TextBox
                {
                    Height = 20,
                    Width = 20,
                    Name = "numLevels",
                    IsReadOnly = true,
                    HorizontalAlignment = HorizontalAlignment.Right
                    
                };

                var button = new Button
                {
                    Content = "Create levels",
                    Width = 80,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                button.Click += ButtonOnClick;
                StackPanel sp = new StackPanel();
                sp.Margin = new Thickness(10);
                Grid.SetRow(sp, i + 1);
                Grid.SetColumn(sp, 1);
                var labelList = new ListBox { MaxHeight = 45 };
                
                sp.Children.Add(labelList);
                sp.Children.Add(button);

                //checkbox for isRandomized
                var checkbox = new CheckBox
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Grid.SetRow(checkbox, i + 1);
                Grid.SetColumn(checkbox, 2);
                


                //combobox for within/between subjects selection
                var cb = new ComboBox
                {
                    Width = 150,
                    Height = 20
                };

                cb.Items.Add("Within Subjects Factor");
                cb.Items.Add("Between Subjects Factor");
                Grid.SetRow(cb, i+1); // I guess this means that "whatever grid you put me in, I'll be in this row and this column. 
                Grid.SetColumn(cb, 3);

                
                //if we already have data, put it in the grid for editing.
                if (_controller.NumFactors > i)
                {
                    tb.Text = _controller.FactorSet.Rows[i]["Name"].ToString();
                    tb2.Text = _controller.FactorSet.Rows[i]["Levels"].ToString();
                    checkbox.IsChecked = _controller.FactorSet.Rows[i].Field<bool>("IsRandomized");
                    cb.SelectedItem = _controller.FactorSet.Rows[i].Field<bool>("IsWithinSubjects") ? "Within Subjects Factor" : "Between Subjects Factor";
                    labelList.ItemsSource = (List<string>) _controller.FactorSet.Rows[i]["Labels"];
                    listOfAllLabels[i] = (List<string>) _controller.FactorSet.Rows[i]["Labels"];

                }

                factorGrid.Children.Add(tb);
                factorGrid.Children.Add(sp);
                factorGrid.Children.Add(checkbox);
                factorGrid.Children.Add(cb);

            }


        }

        private void ButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            //get the row number and label of the button we clicked
            var rowNum = Grid.GetRow((sender as FrameworkElement).Parent as UIElement);
          
            //now get the listbox so we can reset the labels later
            var label = (sender as FrameworkElement).Parent.FindVisualChildren<ListBox>().First(l => l.GetType() == typeof(ListBox));

            List<string> newLabels = new List<string>();
            if (listOfAllLabels[rowNum-1] != null)
                newLabels = listOfAllLabels[rowNum-1];

            AddFactorLabels afl = new AddFactorLabels(newLabels);
            afl.LabelReturn += (fromChild) => newLabels = fromChild;
            afl.ShowDialog();
            if (label != null)
            {
                label.ItemsSource = null;
                label.ItemsSource = newLabels;
            }

            listOfAllLabels[rowNum-1] = newLabels;
            //if (numLevels != null)
            //    numLevels.Text = listOfAllLabels[rowNum - 1].Count.ToString();
        }

        private string CreateLabel(List<string> labels)
        {
            // concatenates all the labels in the list together, separated by a comma.
            string temp = labels.Aggregate("", (current, newLabel) => current + (newLabel + ", "));

            if (temp.Length > 1)
                temp = temp.Remove(temp.Length - 2);
            return temp;
        }

        private void setHeaderProperties(TextBlock element, string name, int row, int column)
        {
            element.Text = name;
            element.FontSize = 14;
            element.FontWeight = FontWeights.Bold;
            element.VerticalAlignment = VerticalAlignment.Center;
            element.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
            factorGrid.Children.Add(element);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (_controller.ModelHasData)
                _controller.ClearFactors(); //resets the research factors, needed for updates

            //add data from the grid to the model
            for (int i = 1; i <= _numFactors; i++)
            {
                var itemsInFirstRow = factorGrid.Children.Cast<UIElement>().Where(a => Grid.GetRow(a) == i);
 
                ResearchFactor newFactor = new ResearchFactor();
                foreach (UIElement uie in itemsInFirstRow)
                {
                    if (uie is TextBox)
                    {
                        if ((uie as TextBox).Name == "factorName")
                            newFactor.Name = (uie as TextBox).Text;
                        //else
                            //newFactor.Levels = int.Parse((uie as TextBox).Text);
                    }
                    else if (uie is ComboBox)
                    {
                        if ((uie as ComboBox).Text == "Within Subjects Factor")
                            newFactor.isWithinSubjects = true;
                    }
                    else if (uie is CheckBox)
                    {
                        newFactor.IsRandomized = (bool)(uie as CheckBox).IsChecked;
                    }
                    else if (uie is StackPanel)
                    {
                        newFactor.Labels = listOfAllLabels[i - 1];
                        //newFactor.Levels = int.Parse((
                        //    (uie as FrameworkElement).FindVisualChildren<TextBox>().First(l => l.GetType() == typeof(TextBox))).Text);
                        newFactor.Levels = listOfAllLabels[i - 1].Count;
                    }
                }

                _controller.addFactor(newFactor);
                
            }
            
            Close();
        }

    }


}
