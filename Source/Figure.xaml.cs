using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    public class Figure : UIElement
    {        
        private readonly int[,,,] _shape = new int[7,4,2,4]
                                               {
                                                   {
                                                       {
                                                           {0, 1, 2, 2},
                                                           {0, 0, 0, 1}
                                                       },
                                                       {
                                                           {0, 1, 1, 1},
                                                           {2, 0, 1, 2}
                                                       },
                                                       {
                                                           {0, 0, 1, 2},
                                                           {0, 1, 1, 1}
                                                       },
                                                       {
                                                           {0, 0, 0, 1},
                                                           {0, 1, 2, 0}
                                                       }
                                                   },
                                                   {
                                                       {
                                                           {0, 1, 1, 2},
                                                           {0, 0, 1, 0}
                                                       },
                                                       {
                                                           {0, 1, 1, 1},
                                                           {1, 0, 1, 2}
                                                       },
                                                       {
                                                           {0, 1, 1, 2},
                                                           {1, 0, 1, 1}
                                                       },
                                                       {
                                                           {0, 0, 0, 1},
                                                           {0, 1, 2, 1}
                                                       }
                                                   },
                                                   {
                                                       {
                                                           {0, 0, 1, 2},
                                                           {0, 1, 0, 0}
                                                       },
                                                       {
                                                           {0, 1, 1, 1},
                                                           {0, 0, 1, 2}
                                                       },
                                                       {
                                                           {0, 1, 2, 2},
                                                           {1, 1, 0, 1}
                                                       },
                                                       {
                                                           {0, 0, 0, 1},
                                                           {0, 1, 2, 2}
                                                       }
                                                   },
                                                   {
                                                       {
                                                           {0, 1, 1, 2},
                                                           {1, 0, 1, 0}
                                                       },
                                                       {
                                                           {0, 0, 1, 1},
                                                           {0, 1, 1, 2}
                                                       },
                                                       {
                                                           {0, 1, 1, 2},
                                                           {1, 0, 1, 0}
                                                       },
                                                       {
                                                           {0, 0, 1, 1},
                                                           {0, 1, 1, 2}
                                                       }
                                                   },
                                                   {
                                                       {
                                                           {0, 1, 2, 3},
                                                           {0, 0, 0, 0}
                                                       },
                                                       {
                                                           {0, 0, 0, 0},
                                                           {0, 1, 2, 3}
                                                       },
                                                       {
                                                           {0, 1, 2, 3},
                                                           {0, 0, 0, 0}
                                                       },
                                                       {
                                                           {0, 0, 0, 0},
                                                           {0, 1, 2, 3}
                                                       }
                                                   },
                                                   {
                                                       {
                                                           {0, 0, 1, 1},
                                                           {0, 1, 0, 1}
                                                       },
                                                       {
                                                           {0, 0, 1, 1},
                                                           {0, 1, 0, 1}
                                                       },
                                                       {
                                                           {0, 0, 1, 1},
                                                           {0, 1, 0, 1}
                                                       },
                                                       {
                                                           {0, 0, 1, 1},
                                                           {0, 1, 0, 1}
                                                       }
                                                   },
                                                   {
                                                       {
                                                           {0, 1, 1, 2},
                                                           {0, 0, 1, 1}
                                                       },
                                                       {
                                                           {0, 0, 1, 1},
                                                           {1, 2, 0, 1}
                                                       },
                                                       {
                                                           {0, 1, 1, 2},
                                                           {0, 0, 1, 1}
                                                       },
                                                       {
                                                           {0, 0, 1, 1},
                                                           {1, 2, 0, 1}
                                                       }
                                                   }
                                               };

        private int ShapeNumber { get; set; }
        public int Offset { get; private set; }
        public int Left { get; set; }
        public int Top { get; private set; }

        private readonly Collection<SolidColorBrush> Color = new Collection<SolidColorBrush> {
            new SolidColorBrush(Colors.DarkMagenta),
            new SolidColorBrush(Colors.HotPink),
            new SolidColorBrush(Colors.Yellow),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.RoyalBlue),
            new SolidColorBrush(Colors.Chartreuse),
            new SolidColorBrush(Colors.LightSlateGray),
            new SolidColorBrush(Colors.ForestGreen)};

        public readonly Collection<TextBlock> Blocks = new Collection<TextBlock>();

        public int RotationId;

        public int Width
        {
            get { return GetTotalLength(0); }
        }

        public int Height
        {
            get { return GetTotalLength(1); }
        }

        private int GetTotalLength(int index)
        {
            int max = _shape[ShapeNumber, RotationId, index, 0];

            for (int i = 1; i < 4; i++)
            {
                max = Math.Max(max, _shape[ShapeNumber, RotationId, index, i]);
            }

            return (max + 1)*Offset;
        }

        public Figure(int shapenumber, Canvas canvas, int width, int height, SolidColorBrush color, int offset)
        {
            ShapeNumber = shapenumber;

            for (int i = 0; i < 4; i++)
            {
                Blocks.Add(new TextBlock
                               {
                                   Background = color,
                                   Width = width,
                                   Height = height
                               });
                canvas.Children.Add(Blocks[i]);
            }

            Offset = offset;
        }

        public Figure(int shapenumber, Canvas canvas, int width, int height,  int offset)
        {
            ShapeNumber = shapenumber;

            for (int i = 0; i < 4; i++)
            {
                Blocks.Add(new TextBlock
                {
                    Background = Color[shapenumber],
                    Width = width,
                    Height = height
                });
                canvas.Children.Add(Blocks[i]);
            }

            Offset = offset;
        }

        public int GetLeftOffset(int i, int rotationId)
        {
            return Offset*_shape[ShapeNumber, rotationId, 0, i];
        }

        public int GetTopOffset(int i, int rotationId)
        {
            return Offset*_shape[ShapeNumber, rotationId, 1, i];
        }

        public void SetLocation(int left, int top)
        {
            for (int i = 0; i < 4; i++)
            {
                Canvas.SetLeft(Blocks[i], left + GetLeftOffset(i, RotationId));
                Canvas.SetTop(Blocks[i], top + GetTopOffset(i, RotationId));
            }

            Left = left;
            Top = top;
        }
    }
}