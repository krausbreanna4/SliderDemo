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

        //constructor
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
                    GridItem item;
                    if (counter == 16)
                    {
                        item = new GridItem(new GridPosition(row, col), "imageEmpty"); //counter is the text that will show up on the gridItem
                        item.ImgPath = "16";
                    }

                    else
                    {
                         item = new GridItem(new GridPosition(row, col),
                            counter.ToString()); //counter is the text that will show up on the gridItem
                    }
                    var tapRecognizer = new TapGestureRecognizer(); //these next three lines allow you to tap on the gridItem and move randomly
                    tapRecognizer.Tapped += OnLabelTapped;
                    item.GestureRecognizers.Add(tapRecognizer); //adding an event handler (tapped recognized) to our gridItem

                    _gridItems.Add(item.CurrentPosition, item);
                    _absoluteLayout.Children.Add(item);

                    counter++;
                }
            }

            Shuffle();

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

            //Did we click on empty? if so, do nothing
            if (item.isEmptySpot() == true)
            {
                return;
            }

            //We know we didnt click on the empty spot

            // Check up, down, left, right, until we find empty
            
            var counter = 0;
            while ( counter < 4)
            {
                GridPosition pos = null;
                if (counter == 0 && item.CurrentPosition.Row != 0)
                {
                    //Get position of square above current item
                    pos = new GridPosition(item.CurrentPosition.Row - 1, item.CurrentPosition.Column); 
                }
               else if (counter == 1 && item.CurrentPosition.Column != SIZE-1)
                {
                    //Get position of square to the right of current item
                    pos = new GridPosition(item.CurrentPosition.Row, item.CurrentPosition.Column + 1);
                }
                else if (counter == 2 && item.CurrentPosition.Row != SIZE-1)
                {
                    //Get position of square below current item
                    pos = new GridPosition(item.CurrentPosition.Row + 1, item.CurrentPosition.Column);
                }
                else if (counter == 3 && item.CurrentPosition.Column != 0)
                {
                    //Get position of square to the left current item
                    pos = new GridPosition(item.CurrentPosition.Row, item.CurrentPosition.Column - 1);
                }

                if (pos != null) //Dont have item to check because of edge
                {

                    GridItem swapWith = _gridItems[pos];
                    if (swapWith.isEmptySpot())
                    {
                        Swap(item, swapWith);
                        break; //if we found the empty spot, break the loop, no need to check further
                    }
                }
                counter = counter + 1;
                OnContentViewSizeChanged(this.Content, null);
            }
            
        }

        void Swap(GridItem item1, GridItem item2)
        {
            //First Swap positions
            GridPosition temp = item1.CurrentPosition;
            item1.CurrentPosition = item2.CurrentPosition;
            item2.CurrentPosition = temp;

            //Then update Dictionary too!
            _gridItems[item1.CurrentPosition] = item1;
            _gridItems[item2.CurrentPosition] = item2;
        }

        void Shuffle()
        {
            Random rand = new Random();
            for (var row=0; row<SIZE; row++)
            {
                for (var col=0; col<SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];

                    int swapRow = rand.Next(0,4);
                    int swapCol = rand.Next(0, 4);
                    GridItem swapItem = _gridItems[new GridPosition(swapRow, swapCol)];

                    Swap(item, swapItem);
                }
            }
        }
        internal class GridItem : Image //by extending :Label gives inheritence, allowing text, etc. and allows the grid item to be aware of its location
    {
            public GridPosition CurrentPosition
            {
                get; set;
            }

            private GridPosition _finalPosition;
            private Boolean _isEmptySpot;

            public String ImgPath
            {
                get; set;
            }

            public GridItem(GridPosition position, String text )//Boolean isEmptySpot = false)
            {
                _finalPosition = position;
                CurrentPosition = position;
                Source = ImageSource.FromResource(
                    "SliderDemo.images.image" + text +".jpeg");
                if(text.Equals("imageEmpty"))
                {
                    _isEmptySpot = true;
                    Source = ImageSource.FromResource(
                    "SliderDemo.images." + text + ".jpg");
                }
                else
                {
                    _isEmptySpot = false;
                    Source = ImageSource.FromResource(
                    "SliderDemo.images.image" + text + ".jpeg");
                }
                HorizontalOptions = LayoutOptions.FillAndExpand;
                VerticalOptions = LayoutOptions.FillAndExpand;
            }

            public Boolean isEmptySpot()
            {
                return _isEmptySpot;

            }

            public void showImgPath()
            {
                if (isEmptySpot())
                {
                    Source = ImageSource.FromResource(this.ImgPath);
                }
            }

            public Boolean isPositionCorrect()
            {
                return _finalPosition.Equals(CurrentPosition);
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
