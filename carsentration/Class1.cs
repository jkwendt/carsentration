using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace carsentration
{
    class Class1
    {
        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new CarsentrationWindow());
        }
    }

    enum CellState { Hidden, Showing, Cleared }
    enum GameState { NoneShowing, OneShowing, TwoShowing, Over }

    class CarsentrationWindow : Window
    {
        // constants
        const int MATCHES = 2;
        const int NROWS = 4;
        const int NCOLS = 7;
        const int NUMBER_CELLS = NROWS * NCOLS;
        const int DISTNTIMG = NUMBER_CELLS / MATCHES;

        // Game state
        CellState[] cellStates;
        GameState gameState = GameState.NoneShowing;
        int numberHiddenCells = NUMBER_CELLS;

        // images
        BitmapImage[] bitmapImages;
        Image[] images;
        BitmapImage clearedBitmap;
        BitmapImage hiddenBitmap;

        // Used to reorder images
        int[] imagesOrderIndices;


        DispatcherTimer timer = new DispatcherTimer();

        // Indices of images that are currently showing
        int showingImageIndex1 = -1, showingImageIndex2 = -1;


        // Constructor builds user interface and initializes 
        // all variables, sets up event handlers
        public CarsentrationWindow()
        {
        
            //Instantiates and initializes hiddenBitmap variable
            hiddenBitmap = new BitmapImage();
            hiddenBitmap.BeginInit();
            hiddenBitmap.UriSource = new Uri("pack://application:,,/Images/hidden.png");
            hiddenBitmap.EndInit();

            //Instantiates and initializes clearedBitmap variable
            clearedBitmap = new BitmapImage();
            clearedBitmap.BeginInit();
            clearedBitmap.UriSource = new Uri(String.Format("pack://application:,,/Images/cleared.png"));
            clearedBitmap.EndInit();


            //Loads Random images into Images array
            Random rand = new Random();
            imagesOrderIndices = new int[DISTNTIMG];


            for (int i = 0; i < imagesOrderIndices.Length; i++)
            {
                int rInt = rand.Next(0, DISTNTIMG + 1);

                while (imagesOrderIndices.Contains(rInt))
                {
                    rand = new Random();
                    rInt = rand.Next(0, DISTNTIMG + 1);

                }
                imagesOrderIndices[i] = rInt;
                //System.Console.Write("Col:" + i + " : " + imagesOrderIndices[i] + "\n");

            }

            //Insantiates bitmapImages array 
            bitmapImages = new BitmapImage[NUMBER_CELLS];

            //Initializes bitmapImages array with images
            for (int i = 0, k = 0 ; i < bitmapImages.Length; i++, k++)
            {
                if (k >= imagesOrderIndices.Length)
                {
                    //System.Console.Write(k +"\n");
                    k = 0;
                }
                bitmapImages[i] = new BitmapImage();
                bitmapImages[i].BeginInit();
                bitmapImages[i].UriSource = new Uri(String.Format("pack://application:,,/Images/car{0}.png", (imagesOrderIndices[k] - 1)));
                bitmapImages[i].EndInit();

            }


            // Create a grid with  NCOLS columns and NROWS
            Grid grid = new Grid();
            grid.ShowGridLines = true;
            
            for (int r = 0; r < NROWS; r++)
            {

                for (int c = 0; c < NCOLS; c++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }
                grid.RowDefinitions.Add(new RowDefinition());
                
            }

//re analyze this and check if need to add this image array to grid
            images = new Image[NUMBER_CELLS];

            //Initializes a  images array with images
            for (int i = 0, k = 0 ; i < images.Length; i++, k++)
            {

                images[i] = new Image();
                images[i].Margin = new Thickness(5);
                images[i].Stretch = System.Windows.Media.Stretch.None;
                /*if (k >= imagesOrderIndices.Length)
                {
                    //System.Console.Write(k +"\n");
                    k = 0;
                }*/
                //System.Console.Write(imagesOrderIndices[k] - 1+"\n");
                images[i].Source = bitmapImages[i];


            }

                //Adds Images to grid
            int n = 0;
            for (int r = 1; r <= NROWS; r++)
            {
                //System.Console.WriteLine(n);
                for (int c = 1; c <= NCOLS; c++, n++)
                {
                    
                    Image image = new Image();
                    image.Margin = new Thickness(5);
                    image.Stretch = System.Windows.Media.Stretch.Uniform;
                    image.Tag = n;


                    image.Source = hiddenBitmap;

                    // Add the image to the grid, specifying  its row and 
                    // column in the grid.
                    grid.Children.Add(image);
                    Grid.SetRow(image, r-1);
                    Grid.SetColumn(image, c - 1);
                    //grid.MouseDown += new MouseButtonEventHandler
                    System.Console.WriteLine(n);
                }
                //System.Console.WriteLine(n);
            }

           
            // Handler for MouseEvent on images
            this.AddHandler(MouseDownEvent, new MouseButtonEventHandler(CarsentrationWindow_MouseDown));
            // Handler for timer
            this.timer.Interval = new TimeSpan(0, 0, 0, 2);
            this.timer.Tick += timer_Tick;

            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.Content = grid;
            this.Title = "Image and Grid Example";
            Application.Current.Properties["mde"] = ;
           // this.ResizeMode = ResizeMode.NoResize;

            
            
        }

        // Event handler for the timer
        void timer_Tick(object sender, EventArgs e)
        {
            //Image img = Application.Current.Properties["grd"] as Image;
            //img.Source = hiddenBitmap;

        }

        // Event handler for the mouse
        void CarsentrationWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {   Image img = new Image();
            
            img = (Image)e.OriginalSource;
            int i = (int)img.Tag;
            
           // images[i] = 
            img.Source = bitmapImages[i];
            if(gameState == GameState.NoneShowing)
            {
                img.Source = bitmapImages[i];
                //cellStates[i] = CellState.Showing;
                gameState = GameState.OneShowing;
                showingImageIndex1 = i;
            }
            else if(gameState == GameState.OneShowing)
            {
                img.Source = bitmapImages[i];
                //cellStates[i] = CellState.Showing;
                showingImageIndex2 = i;
                gameState = GameState.TwoShowing;
                timer.Start();
            }

            
            System.Console.WriteLine(i);



    
        }
    }

}