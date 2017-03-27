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
            GridItem item = (GridItem)sender;

            Random rand = new Random();
            int move = rand.Next(0, 4);

            //Adjust random move to account for edges
            if (move == 0 && item.Position.Row == 0)
            {
                move = 2;
            }

            else if (move== 1 && item.Position.Column == SIZE - 1)
            {
                move = 3;
            }

            else if (move == 2 && item.Position.Row == SIZE - 1)
            {
                move = 0;
            }

            else if (move == 3 && item.Position.Column == 0)
            {
                move = 1;
            }

            int row = 0;
            int col = 0;

            if (move == 0) //Move Up
            {
                row = item.Position.Row - 1;
                col = item.Position.Column;
            }

            else if (move == 1) //Move Right
            {
                row = item.Position.Row;
                col = item.Position.Column + 1;
            }
            else if (move == 2) //Move DOWN
            {
                row = item.Position.Row + 1;
                col = item.Position.Column;
            }
            else //Move Left
            {
                row = item.Position.Row;
                col = item.Position.Column - 1;
            }

            GridItem swapWith = _gridItems[new GridPosition(row, col)];
            Swap(item, swapWith);
            OnContentViewSizeChanged(this.Content, null);
            //throw new NotImplementedException();
        }

        void Swap(GridItem item1, GridItem item2)
        {
            //First Swap positions
            GridPosition temp = item1.Position;
            item1.Position = item2.Position;
            item2.Position = temp;

            //Then update Dictionary too!
            _gridItems[item1.Position] = item1;
            _gridItems[item2.Position] = item2;
        }

        internal class GridItem : Image //by extending :Label gives inheritence, allowing text, etc. and allows the grid item to be aware of its location
    {
            public GridPosition Position
            {
                get; set;
            }

            public GridItem(GridPosition position, String text)
            {
                Position = position;
                Source = ImageSource.FromResource(
                    "SliderDemo.images.image" + text +".jpeg");
                
                HorizontalOptions = LayoutOptions.FillAndExpand;
                VerticalOptions = LayoutOptions.FillAndExpand;
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
