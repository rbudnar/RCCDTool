﻿using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Converters;
using Xceed.Wpf.DataGrid.ValidationRules;
using Xceed.Wpf.DataGrid.Views;
using DataRow = System.Data.DataRow;
//using Xceed.Wpf.DataGrid.Settings;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace RCCDTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _designSelection;
        private IController _controller;

        public DataTable FactorSet
        {
            get
            {
                if (_controller?.ModelHasData ?? false)
                    return _controller.FactorSet;
                else
                    return new DataTable();
                
            }
        }
        public MainWindow()
        {
            //InitializeComponent();
            
        }
        public MainWindow(string designSelection, IModel model, IController controller)
        {
            
            InitializeComponent();
            
            _designSelection = designSelection;
            designLabel.Content += designSelection;
            _controller = controller;
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int nPerGroupCalc = (Int32.Parse(totalN.Text) / Int32.Parse(numFactors.Text));
            nPerGroup.Text = nPerGroupCalc.ToString();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int nf = int.Parse(numFactors.Text);
                EditFactors ef = new EditFactors(nf, _controller);
               
                ef.Show();
                FactorsGrid.Items.Refresh();
            }
            catch (FormatException exception)
            {
                MessageBox.Show("Invalid entry for number of factors! Please enter an integer greater than 1.");
                MessageBox.Show("Exception was:" + exception);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception was:" + exception);
            }
        }
    }
}
