using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace WindsOfWar
{
    public static class SimpleFont
    {
        private static Dictionary<char, bool[,]> _glyphs = new Dictionary<char, bool[,]>();
        private static bool _initialized = false;

        public static void Initialize()
        {
            if (_initialized) return;

            // 3x5 font definitions
            AddGlyph('A', new string[] {
                " X ",
                "X X",
                "XXX",
                "X X",
                "X X"
            });
            AddGlyph('B', new string[] {
                "XX ",
                "X X",
                "XX ",
                "X X",
                "XX "
            });
            AddGlyph('C', new string[] {
                " XX",
                "X  ",
                "X  ",
                "X  ",
                " XX"
            });
            AddGlyph('D', new string[] {
                "XX ",
                "X X",
                "X X",
                "X X",
                "XX "
            });
            AddGlyph('E', new string[] {
                "XXX",
                "X  ",
                "XXX",
                "X  ",
                "XXX"
            });
            AddGlyph('F', new string[] {
                "XXX",
                "X  ",
                "XXX",
                "X  ",
                "X  "
            });
            AddGlyph('G', new string[] {
                " XX",
                "X  ",
                "X X",
                "X X",
                " XX"
            });
            AddGlyph('H', new string[] {
                "X X",
                "X X",
                "XXX",
                "X X",
                "X X"
            });
            AddGlyph('I', new string[] {
                "XXX",
                " X ",
                " X ",
                " X ",
                "XXX"
            });
            AddGlyph('J', new string[] {
                "  X",
                "  X",
                "  X",
                "X X",
                " X "
            });
            AddGlyph('K', new string[] {
                "X X",
                "X X",
                "XX ",
                "X X",
                "X X"
            });
            AddGlyph('L', new string[] {
                "X  ",
                "X  ",
                "X  ",
                "X  ",
                "XXX"
            });
            AddGlyph('M', new string[] {
                "X X",
                "XXX",
                "X X",
                "X X",
                "X X"
            });
            AddGlyph('N', new string[] {
                "XX ",
                "X X",
                "X X",
                "X X",
                "X X"
            });
            AddGlyph('O', new string[] {
                " X ",
                "X X",
                "X X",
                "X X",
                " X "
            });
            AddGlyph('P', new string[] {
                "XX ",
                "X X",
                "XX ",
                "X  ",
                "X  "
            });
            AddGlyph('Q', new string[] {
                " X ",
                "X X",
                "X X",
                " XX",
                "  X"
            });
            AddGlyph('R', new string[] {
                "XX ",
                "X X",
                "XX ",
                "X X",
                "X X"
            });
            AddGlyph('S', new string[] {
                " XX",
                "X  ",
                " X ",
                "  X",
                "XX "
            });
            AddGlyph('T', new string[] {
                "XXX",
                " X ",
                " X ",
                " X ",
                " X "
            });
            AddGlyph('U', new string[] {
                "X X",
                "X X",
                "X X",
                "X X",
                " XX"
            });
            AddGlyph('V', new string[] {
                "X X",
                "X X",
                "X X",
                " X ",
                " X "
            });
            AddGlyph('W', new string[] {
                "X X",
                "X X",
                "X X",
                "XXX",
                "X X"
            });
            AddGlyph('X', new string[] {
                "X X",
                " X ",
                " X ",
                " X ",
                "X X"
            });
            AddGlyph('Y', new string[] {
                "X X",
                "X X",
                " X ",
                " X ",
                " X "
            });
            AddGlyph('Z', new string[] {
                "XXX",
                "  X",
                " X ",
                "X  ",
                "XXX"
            });
            AddGlyph('0', new string[] {
                "XXX",
                "X X",
                "X X",
                "X X",
                "XXX"
            });
            AddGlyph('1', new string[] {
                " X ",
                "XX ",
                " X ",
                " X ",
                "XXX"
            });
            AddGlyph('2', new string[] {
                "XXX",
                "  X",
                "XXX",
                "X  ",
                "XXX"
            });
            AddGlyph('3', new string[] {
                "XXX",
                "  X",
                "XXX",
                "  X",
                "XXX"
            });
            AddGlyph('4', new string[] {
                "X X",
                "X X",
                "XXX",
                "  X",
                "  X"
            });
            AddGlyph('5', new string[] {
                "XXX",
                "X  ",
                "XXX",
                "  X",
                "XXX"
            });
            AddGlyph('6', new string[] {
                "XXX",
                "X  ",
                "XXX",
                "X X",
                "XXX"
            });
            AddGlyph('7', new string[] {
                "XXX",
                "  X",
                "  X",
                "  X",
                "  X"
            });
            AddGlyph('8', new string[] {
                "XXX",
                "X X",
                "XXX",
                "X X",
                "XXX"
            });
            AddGlyph('9', new string[] {
                "XXX",
                "X X",
                "XXX",
                "  X",
                "XXX"
            });
            AddGlyph('.', new string[] {
                "   ",
                "   ",
                "   ",
                "   ",
                " X "
            });
            AddGlyph(':', new string[] {
                "   ",
                " X ",
                "   ",
                " X ",
                "   "
            });
            AddGlyph('-', new string[] {
                "   ",
                "   ",
                "XXX",
                "   ",
                "   "
            });
             AddGlyph('!', new string[] {
                " X ",
                " X ",
                " X ",
                "   ",
                " X "
            });

            _initialized = true;
        }

        private static void AddGlyph(char c, string[] rows)
        {
            int width = rows[0].Length;
            int height = rows.Length;
            bool[,] grid = new bool[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (rows[y][x] != ' ')
                    {
                        grid[x, y] = true;
                    }
                }
            }
            _glyphs[c] = grid;
        }

        public static void DrawString(SpriteBatch spriteBatch, Texture2D texture, string text, Vector2 position, Color color, int scale = 1)
        {
            if (!_initialized) Initialize();

            int cursorX = 0;
            int cursorY = 0;

            foreach (char c in text.ToUpper())
            {
                if (c == '\n')
                {
                    cursorX = 0;
                    cursorY += 6 * scale; // Line height
                    continue;
                }

                if (_glyphs.TryGetValue(c, out bool[,] grid))
                {
                    int width = grid.GetLength(0);
                    int height = grid.GetLength(1);

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            if (grid[x, y])
                            {
                                spriteBatch.Draw(texture, new Rectangle((int)position.X + cursorX + x * scale, (int)position.Y + cursorY + y * scale, scale, scale), color);
                            }
                        }
                    }
                    cursorX += (width + 1) * scale; // Add spacing
                }
                else
                {
                    cursorX += 4 * scale; // Space for unknown char
                }
            }
        }
    }
}
