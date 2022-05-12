﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using EvolveRentals.iOS;
using EvolveRentals.Renders;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessTimePicker), typeof(BorderlessTimePickerRenderer))]

namespace EvolveRentals.iOS
{
    public class BorderlessTimePickerRenderer : TimePickerRenderer
    {
        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {

                Control.Layer.BorderWidth = 0;
                Control.BorderStyle = UITextBorderStyle.None;

                var element = e.NewElement as BorderlessTimePicker;
                if (!string.IsNullOrWhiteSpace(element.Placeholder))
                {
                    Control.Text = element.Placeholder;
                }

                Control.ShouldEndEditing += (textField) =>
                {
                    var seletedDate = (UITextField)textField;
                    var text = seletedDate.Text;
                    if (text == element.Placeholder)
                    {
                        Control.Text = DateTime.Now.TimeOfDay.ToString();
                    }
                    return true;
                };
            }
        }
    }
}