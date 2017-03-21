using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace SliderDemo
{
    public class SliderDemoPage : ContentPage
    {

        //Properties
        private const int SIZE = 4;

        private AbsoluteLayout _absoluteLayout; //layout of the application
        private Dictionary<GridPosition, GridItem> _gridItems; //Dictionary allows Keys .. Click the lightblub to generate GridPosition and GridItem as class

        public SliderDemoPage()
        {
            _gridItems = new Dictionary<GridPosition, GridItem>();
            _absoluteLayout = new AbsoluteLayout
            {
                BackgroundColor = Color.Blue,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center

            };

            var counter = 1; // start the counter at one for the squares to be labled starting at 1
            for (var row = 0; row < SIZE; row++) //using SIZE, gives you the ability to change the size of the grid in the future...
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item = new GridItem(new GridPosition(row, col),
                        counter.ToString()); //counter is the text that will show up on the gridItem

                    var tapRecognizer = new TapGestureRecognizer(); //these next three lines allow you to tap on the gridItem and move randomly
                    tapRecognizer.Tapped += OnLabelTapped;
                    item.GestureRecognizers.Add(tapRecognizer); //adding an event handler (tapped recognized) to our gridItem

                    _gridItems.Add(item.Position, item);
                    _absoluteLayout.Children.Add(item);

                    counter++;
                }
            }

            ContentView contentView = new ContentView
            {
                Content = _absoluteLayout
            };
            contentView.SizeChanged += OnContentViewSizeChanged;
            this.Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5);
            this.Content = contentView;
        }



        private void OnContentViewSizeChanged(object sender, EventArgs e)
        {
            ContentView contentView = (ContentView)sender;
            double squareSize = Math.Min(contentView.Width, contentView.Height) / SIZE;

            for (var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];
                    Rectangle rect = new Rectangle(col * squareSize, row * squareSize, squareSize, squareSize);
                    AbsoluteLayout.SetLayoutBounds(item, rect);
                }
            }
        }

        private void OnLabelTapped(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        internal class GridItem : Label //by extending :Label gives inheritence, allowing text, etc. and allows the grid item to be aware of its location
    {
            public GridPosition Position
            {
                get; set;
            }

            public GridItem(GridPosition position, String text)
            {
                Position = position;
                Text = text;
                TextColor = Color.White;
                HorizontalOptions = LayoutOptions.Center;
                VerticalOptions = LayoutOptions.Center;
            }
    }

    internal class GridPosition
    {
        public int Row
        {
            get; set;
        }

        public int Column
        {
            get; set;
        }

        public GridPosition (int row, int col)
        {
            Row = row;
            Column = col;
        }

        public override bool Equals(object obj)
        {
            GridPosition other = obj as GridPosition; // asks if it is a grid position, other will come back as null
            if (other != null && this.Row == other.Row && this.Column == other.Column)
                {
                    return true;
                }
            return false;
        }

            public override int GetHashCode()
            {
                return 17 * (23 + this.Row.GetHashCode()) * (23 + this.Column.GetHashCode()); //always use prime numbers
            }
        }
}
}
