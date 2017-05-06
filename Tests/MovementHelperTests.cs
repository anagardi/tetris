using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Windows.Controls;
using Tetris;

namespace Tests
{
    [TestClass]
    public class MovementHelperTests
    {
        private TestContext _testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
            }
        }

        private Canvas _canvas; // main canvas sample, for test figures and situations generation
        private int _width; // width of grid cell 

        private int _lines; // gained lines count
        private int _score; // score value

        [TestInitialize]
        public void TestInitialize()
        {
            _canvas = new Canvas
            {
                Width = 300,
                Height = 450
            };

            _width = 25;
            _lines = 0;
            _score = 0;
        }

        [TestMethod()] //Test score for zero complete line
        public void CalculateScoreForZeroLineTest()
        {

            int count = (int)_canvas.Width / (4 * _width) - 1;

            for (int i = 0; i < count; i++)
            {
                //Creaet figure "I" - four blocks next to each other, like xxxx
                Figure figure = new Figure(4, _canvas, _width, _width, _width);
                figure.SetLocation(i * figure.Width, (int)_canvas.Height - figure.Height);
                _score = MovementHelper.CalculateScore(figure, _canvas, _score, out _lines);
                Assert.IsTrue(_lines == 0);
                Assert.IsTrue(_score == _lines * MovementHelper.DefaultUnits);                
            }
        }

        [TestMethod()] //Test score for one complete line
        public void CalculateScoreForOneLineTest()
        {

            int count = (int)_canvas.Width / (4 * _width);

            for (int i = 0; i < count; i++)
            {
                //Creaet figure "I" - four blocks next to each other, like xxxx
                Figure figure = new Figure(4, _canvas, _width, _width, _width);
                figure.SetLocation(i * figure.Width, (int)_canvas.Height - figure.Height);
                _score = MovementHelper.CalculateScore(figure, _canvas, _score, out _lines);
                if (i < count - 1)
                {
                    Assert.IsTrue(_lines == 0);
                    Assert.IsTrue(_score == 0);
                }
                else
                {
                    Assert.IsTrue(_lines == 1);
                    Assert.IsTrue(_score == _lines * MovementHelper.DefaultUnits);
                }
            }
        }

        [TestMethod()] //Test score for two complete lines
        public void CalculateScoreForTwoLinesTest()
        {
            int count = (int)_canvas.Width / (2 * _width);

            for (int i = 0; i < count; i++)
            {
                //Creaet figure "O" - two blocks boven other two like square xx
                //                                                           xx
                Figure figure = new Figure(5, _canvas, _width, _width, _width);
                figure.SetLocation(i * figure.Width, (int)_canvas.Height - figure.Height);
                _score = MovementHelper.CalculateScore(figure, _canvas, _score, out _lines);
            }

            Assert.IsTrue(_lines == 2);
            Assert.IsTrue(_score == _lines * MovementHelper.DefaultUnits);
        }

        [TestMethod()] //Test score for three complete lines
        public void CalculateScoreThreeLinesTest()
        {
            int count = (int)_canvas.Width / (4 * _width);

            for (int i = 0; i < count; i++)
            {
                //Creaet figures "I" and "O" to fill the canvas with 3 rows
                Figure figure1 = new Figure(4, _canvas, _width, _width, _width);
                figure1.SetLocation(i * figure1.Width, (int)_canvas.Height - figure1.Height);


                Figure figure2 = new Figure(5, _canvas, _width, _width, _width);
                figure2.SetLocation(i * figure1.Width, (int)_canvas.Height - figure1.Height - figure2.Height);


                Figure figure3 = new Figure(5, _canvas, _width, _width, _width);
                figure3.SetLocation(i * figure1.Width + figure2.Width, (int)_canvas.Height - figure1.Height - figure2.Height);

                _score = MovementHelper.CalculateScore(figure3, _canvas, _score, out _lines);
            }

            Assert.IsTrue(_lines == 3);
            Assert.IsTrue(_score == _lines * MovementHelper.DefaultUnits);
        }


        [TestMethod()] //Test score for four complete lines
        public void CalculateScoreFourLinesTest()
        {
            int count = (int)_canvas.Width / (2 * _width);
            //Create figures "O" to fill the canvas with 4 rows
            for (int i = 0; i < count; i++)
            {
                Figure figure = new Figure(5, _canvas, _width, _width, _width);
                figure.SetLocation(i * figure.Width, (int)_canvas.Height - figure.Height);

                Figure figure2 = new Figure(5, _canvas, _width, _width, _width);
                figure2.SetLocation(i * figure2.Width, (int)_canvas.Height - figure2.Height - figure2.Height);

                _score = MovementHelper.CalculateScore(figure2, _canvas, _score, out _lines);
            }

            Assert.IsTrue(_lines == 4);
            Assert.IsTrue(_score == MovementHelper.TetrisUnits);
        }

        /// <summary>
        /// First set the figure to the canvas' left top corner
        /// Then set the figure to the bottom of the canvas
        /// </summary>
        [TestMethod()]
        public void CanMoveDownTest()
        {
            for (int i = 0; i < 7; i++)
            {
                Figure figure = new Figure(i, _canvas, _width, _width, _width);
                figure.SetLocation(0, 0);
                Assert.IsTrue(MovementHelper.CanMoveDown(figure, _canvas));
                figure.SetLocation(0, (int)_canvas.Height - figure.Height);
                Assert.IsFalse(MovementHelper.CanMoveDown(figure, _canvas));
                _canvas.Children.Clear();
            }
        }

        /// <summary>
        /// First set the figure to the canvas' left top corner
        /// Then set the figure to the middle position
        /// </summary>
        [TestMethod()]
        public void CanMoveLeftTest()
        {
            for (int i = 0; i < 7; i++)
            {
                Figure figure = new Figure(i, _canvas, _width, _width, _width);
                figure.SetLocation(0, 0);
                Assert.IsFalse(MovementHelper.CanMoveLeft(figure, _canvas));
                figure.SetLocation((int)_canvas.Width / 2, 0);
                Assert.IsTrue(MovementHelper.CanMoveLeft(figure, _canvas));
                _canvas.Children.Clear();
            }
        }

        /// <summary>
        /// First set the figure to the canvas' right top corner
        /// Then set the figure to the middle position
        /// </summary>
        [TestMethod()]
        public void CanMoveRightTest()
        {
            for (int i = 0; i < 7; i++)
            {
                Figure figure = new Figure(i, _canvas, _width, _width, _width);
                figure.SetLocation((int)_canvas.Width - figure.Width, 0);
                Assert.IsFalse(MovementHelper.CanMoveRight(figure, _canvas));
                figure.SetLocation((int)_canvas.Width / 2, 0);
                Assert.IsTrue(MovementHelper.CanMoveRight(figure, _canvas));
                _canvas.Children.Clear();
            }
        }

        /// <summary>
        /// First set the figure to the open position
        /// Then set the figure between the left border of canvas and group of blocks-wall
        /// </summary>
        [TestMethod()]
        public void CanRotateLeftTest1()
        {
            Figure figureWall = new Figure(4, _canvas, _width, _width, _width);
            figureWall.SetLocation(_width * 2, (int)_canvas.Height - _width * 4);
            MovementHelper.RotateLeft(figureWall, _canvas);

            for (int i = 0; i < 7; i++)
            {
                Figure figure = new Figure(i, _canvas, _width, _width, _width);
                MovementHelper.RotateLeft(figure, _canvas);
                figure.SetLocation(0, (int)_canvas.Height - figureWall.Height - _width);
                Assert.IsFalse(MovementHelper.CanRotateLeft(figure, _canvas));               
                _canvas.Children.Remove(figure);
            }            
        }

        [TestMethod()]
        public void CanRotateLeftTest2()
        {                    
            for (int i = 0; i < 7; i++)
            {
                Figure figure = new Figure(i, _canvas, _width, _width, _width);               
                figure.SetLocation(_width * 4, _width * 4);
                Assert.IsTrue(MovementHelper.CanRotateLeft(figure, _canvas));
                _canvas.Children.Clear();
            }
        }

        /// <summary>
        /// First set the figure to the open position
        /// Then set the figure between the right border of canvas and group of blocks-wall
        /// </summary>
        [TestMethod()]
        public void CanRotateRightTest()
        {
            Figure figureWall = new Figure(4, _canvas, _width, _width, _width);
            figureWall.SetLocation((int)_canvas.Width - _width * 3, (int)_canvas.Height - _width * 4);
            MovementHelper.RotateLeft(figureWall, _canvas);
            for (int i = 0; i < 7; i++)
            {
                Figure figure = new Figure(i, _canvas, _width, _width, _width);
                MovementHelper.RotateLeft(figure, _canvas);
                figure.SetLocation((int)_canvas.Width - _width*2, (int)_canvas.Height - figureWall.Height - _width);
                Assert.IsFalse(MovementHelper.CanRotateRight(figure, _canvas));                
                _canvas.Children.Remove(figure);
            }
        }

        [TestMethod()]
        public void CanRotateRightTest2()
        {
            for (int i = 0; i < 7; i++)
            {
                Figure figure = new Figure(i, _canvas, _width, _width, _width);
                figure.SetLocation(_width * 4, _width * 4);
                Assert.IsTrue(MovementHelper.CanRotateRight(figure, _canvas));
                _canvas.Children.Clear();
            }
        }

        [TestMethod()]
        public void MoveRightTest()
        {
            for (int i = 0; i < 7; i++)
            {
                int currentHorizontalPosition = i*_width;

                Figure figure = new Figure(i, _canvas, _width, _width, _width);
                figure.SetLocation(currentHorizontalPosition, _width * 4);
                MovementHelper.MoveRight(figure, _canvas);
                int newHorizontalPosition = figure.Left;
                Assert.IsTrue(currentHorizontalPosition == newHorizontalPosition - _width);
                //Debug.WriteLine("{0} {1}", currentHorizontalPosition, newHorizontalPosition);
                _canvas.Children.Clear();
            }
        }

        [TestMethod()]
        public void MoveLeftTest()
        {
            for (int i = 0; i < 7; i++)
            {
                int currentHorizontalPosition = (int) _canvas.Width - 4*_width;

                Figure figure = new Figure(i, _canvas, _width, _width, _width);
                figure.SetLocation(currentHorizontalPosition, _width * 4);
                MovementHelper.MoveLeft(figure, _canvas);
                int newHorizontalPosition = figure.Left;
                Assert.IsTrue(currentHorizontalPosition == newHorizontalPosition + _width);
                //Debug.WriteLine("{0} {1}", currentHorizontalPosition, newHorizontalPosition);
                _canvas.Children.Clear();
            }
        }

        [TestMethod()]
        public void RotateRightTest()
        {
            for (int i = 0; i < 7; i++)
            {               
                    int x = 4 * _width;
                    int y = 4 * _width;

                    Figure figure = new Figure(i, _canvas, _width, _width, _width);
                    figure.SetLocation(x,y);
                    int width = figure.Width;
                    MovementHelper.RotateRight(figure, _canvas);
                    Assert.IsTrue(x == figure.Left);
                    Assert.IsTrue(y == figure.Top);
                    Assert.IsTrue(figure.Height == width);                
                    _canvas.Children.Clear();                
            }
        }
        [TestMethod()]
        public void RotateLeftTest()
        {
            for (int i = 0; i < 7; i++)
            {
                int x = 4 * _width;
                int y = 4 * _width;

                Figure figure = new Figure(i, _canvas, _width, _width, _width);
                figure.SetLocation(x, y);
                int width = figure.Width;
                MovementHelper.RotateRight(figure, _canvas);
                Assert.IsTrue(x == figure.Left);
                Assert.IsTrue(y == figure.Top);
                Assert.IsTrue(figure.Height == width);
                _canvas.Children.Clear();
            }
        }
    }
}
