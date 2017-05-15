using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Vaerator.Controls
{
    public class ExtendedFrame : Frame
    {
        public static readonly BindableProperty OutlineWidthProperty = BindableProperty.Create(nameof(OutlineWidth), typeof(Single), typeof(ExtendedFrame), 1.0f,
            validateValue: (bindable, value) => ((float)value) >= 1.0f);

        public Single OutlineWidth
        {
            get { return (Single)GetValue(OutlineWidthProperty); }
            set { SetValue(OutlineWidthProperty, value); }
        }
    }
}
