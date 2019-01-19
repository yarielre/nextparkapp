using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NextPark.Mobile.Controls
{

    public partial class DayView : ContentView
    {

        public RelativeLayout EventLayout
        {
            get { return (RelativeLayout)this.FindByName("RowsView"); }
        }

        public static void AddEvent(BindableObject control, string text) {
            DayView myDayView = (DayView)control;

            // Create event Frame
            Label eventText = new Label();
            eventText.Text = text;
            eventText.FontSize = 12;
            eventText.TextColor = Color.DarkRed;
            eventText.HorizontalTextAlignment = TextAlignment.Start;
            eventText.VerticalTextAlignment = TextAlignment.Start;
            eventText.Margin = new Thickness(10, 2, 0, 0);

            Frame eventFrame = new Frame();
            eventFrame.BackgroundColor = Color.Red;
            eventFrame.CornerRadius = 8;
            eventFrame.HeightRequest = 30;
            eventFrame.Margin = 0;
            eventFrame.Padding = 0;
            eventFrame.Opacity = 0.8;
            eventFrame.Content = eventText;
            eventFrame.AnchorX = 87;

            RelativeLayout layout = myDayView.FindByName<RelativeLayout>("RowsView");

            layout.Children.Add(eventFrame,
                                xConstraint: Constraint.RelativeToParent((parent) => { return parent.X - 20; }),
                                yConstraint: Constraint.RelativeToParent((parent) => { return parent.Y + 7 + 40*2; }),
                                widthConstraint: Constraint.RelativeToParent((parent) => { return parent.Width * 0.8; }));
        }

        public DayView()
        {
            InitializeComponent();
        }
    }
}
