using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EarphoneLeftAndRight.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ValueEntryView : ContentView
	{
		public ValueEntryView()
		{
			InitializeComponent();
		}

		public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(ValueEntryView), 0.0, BindingMode.TwoWay);

		public double Value
		{
			get => (double)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public static readonly BindableProperty MinProperty = BindableProperty.Create(nameof(Min), typeof(double), typeof(ValueEntryView), 0.0, BindingMode.OneWay);

		public double Min
		{
			get => (double)GetValue(MinProperty);
			set => SetValue(MinProperty, value);
		}

		public static readonly BindableProperty MaxProperty = BindableProperty.Create(nameof(Max), typeof(double), typeof(ValueEntryView), 1.0, BindingMode.OneWay);

		public double Max
		{
			get => (double)GetValue(MaxProperty);
			set => SetValue(MaxProperty, value);
		}

		public static readonly BindableProperty DefaultProperty = BindableProperty.Create(nameof(Default), typeof(double), typeof(ValueEntryView), 0.0, BindingMode.OneWay);

		public double Default
		{
			get => (double)GetValue(DefaultProperty);
			set => SetValue(DefaultProperty, value);
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			this.Value = this.Default;
		}
	}
}