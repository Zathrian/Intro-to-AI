using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Project2.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Part1_GeneticAlgorithm : Page
	{
		public Part1_GeneticAlgorithm()
		{
			this.InitializeComponent();
		}

		private void PageHeader_Loaded(object sender, RoutedEventArgs e)
		{
			this.Header.setHeading("Part1: Genetic Algorithm to Solve 3SAT Problem");
		}

		private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch(this.listView.SelectedIndex)
			{
				case 0:
					this.Header.setHeading("25 SAT");
					break;
				case 1:
					this.Header.setHeading("50 SAT");
					break;
				case 2:
					this.Header.setHeading("100 SAT");
					break;
			}
		}
	}
}
