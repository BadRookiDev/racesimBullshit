using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Controller;
using Model;

namespace View
{
    public static class ASCIIvisualizer
    {
        public static void OnDriversChanged(object sender, DriversChangedEventArgs e)
        {
            DrawTrack(e.Track);
        }
        
        #region graphics
        
        private static readonly ImmutableDictionary<SectionTypes, string[]> AsciiSectionGraphicsDict =
            new Dictionary<SectionTypes, string[]>
            {
                { SectionTypes.Finish, new[] { "----", "  1#", " 2 #", "----" } },
                { SectionTypes.StartGrid, new[] { "----", " \u00d71\u00d7", "\u00d72\u00d7 ", "----" } },
                { SectionTypes.Straight, new[] { "----", "  1 ", " 2  ", "----" } },
                { SectionTypes.LeftCorner, new[] { "+  |", " 1 |", "  2|", "---+" } },
                { SectionTypes.RightCorner, new[] { "---+", " 2 |", "  1|", "+  |" } }
            }.ToImmutableDictionary();
        
        #endregion
        
        public static void Initialize()
        {
            Data.CurrentRace.DriversChanged += OnDriversChanged;
            
            DrawTrack(Data.CurrentRace.Track);
        }
        
        public static void DrawTrack(Track track)
        {
            Console.Clear();
            
            var currentDirection = Direction.Right;
            var xPos = 0;
            var yPos = 0;

            List<SectionDetails> sectionDetailsList = new List<SectionDetails>();

            foreach (var section in track.Sections)
            {
                var turnTowards = section.SectionType switch
                {
                    SectionTypes.LeftCorner => -1,
                    SectionTypes.RightCorner => +1,
                    _ => 0
                };
                
                var asciiSection = TransformSectionGraphics(section.SectionType, currentDirection);
                var sectionData = Data.CurrentRace.GetSectionData(section);
                
                sectionDetailsList.Add(new SectionDetails(sectionData, xPos, yPos, asciiSection));

                var nextDirection = GetNextDirection(currentDirection, turnTowards);
                var vectors = GetVectorsFromNextDirection(nextDirection);

                xPos += vectors.xVec;
                yPos += vectors.yVec;

                currentDirection = nextDirection;
            }
            
            var grid = GenerateTrackGrid(sectionDetailsList);
            PrintGrid(grid);
        }

        private static void PrintGrid(SectionDetails[][] grid)
        {
            var numRows = grid[0].Length;
            var numCols = grid.Length;

            for (var y = 0; y < numRows; y++)
            {
                for (var lineIndex = 0; lineIndex < 4; lineIndex++) //eerst elke line horizontaal, dan row opschuiven.
                {
                    for (var x = 0; x < numCols; x++)
                    {
                        var sectionDetails = grid[x][y];
                        if (sectionDetails == null)
                        {
                            Console.Write("****"); //Dit dient als terrein dat niet toegankelijk is. 
                        }
                        else
                        {
                            var asciiLine = sectionDetails.AsciiSection[lineIndex];
                            if (asciiLine.Contains("1") || asciiLine.Contains("2"))
                            {
                                PrintSectionLinePutParticipants(asciiLine, sectionDetails.Data.Left, 
                                    sectionDetails.Data.Right);
                            }
                            else
                            {
                                Console.Write(asciiLine);
                            }   
                        }
                    }
                    Console.WriteLine(" ");
                }
            }
        }
        
        private static void PrintSectionLinePutParticipants(string asciiLine, IParticipant left, IParticipant right)
        {
            (string leftChar, IParticipant leftPar) pcl = ((left?.Name ?? " ")[0].ToString(), left);
            (string rightChar, IParticipant rightPar) pcr = ((right?.Name ?? " ")[0].ToString(), right);
            
            foreach (var character in asciiLine)
            {
                if (character == '1' || character == '2')
                {
                    (var parChar, IParticipant participant) = character == '1' ? pcl : pcr;
                    if (participant != null)
                    {
                        Console.ForegroundColor = 
                            (ConsoleColor)Enum.Parse(typeof(ConsoleColor), participant.TeamColor.ToString());   
                    }
                    Console.Write(parChar);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Write(character);
                }
            }
        }

        private static SectionDetails[][] GenerateTrackGrid(List<SectionDetails> sectionDetailsList)
        {
            var minXPos = sectionDetailsList.Min(section => section.XPos);
            var minYPos = sectionDetailsList.Min(section => section.YPos);
            var maxXPos = sectionDetailsList.Max(section => section.XPos);
            var maxYPos = sectionDetailsList.Max(section => section.YPos);
            
            var width = maxXPos - minXPos + 1;
            var height = maxYPos - minYPos + 1;
            
            var grid = new SectionDetails[width][];
            for (var i = 0; i < width; i++)
            {
                grid[i] = new SectionDetails[height];
            }
            
            foreach (var sectionDetails in sectionDetailsList)
            {
                var gridX = sectionDetails.XPos - minXPos;
                var gridY = sectionDetails.YPos - minYPos;
                grid[gridX][gridY] = sectionDetails;
            }

            return grid;
        }

        
        private static (int xVec, int yVec) GetVectorsFromNextDirection(Direction nextDirection)
        {
            return nextDirection switch
            {
                Direction.Up => (0, -1), //Up is -1 want we willen deze bovenaan in de 2d array
                Direction.Down => (0, 1),
                Direction.Left => (-1, 0),
                Direction.Right => (1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(nextDirection), nextDirection, 
                    "This is an invalid direction!")
            };
        }

        private static Direction GetNextDirection(Direction direction, int turnDirection)
        {   
            var newDirection = (int) direction + turnDirection;
            if (newDirection < 0) { newDirection += 4; }
            newDirection %= 4;

            return (Direction)newDirection;
        }

        #region graphics transformal helper functions
        
        private static string[] TransposeSectionVertical(string[] asciiSection)
        {
            return Enumerable.Range(0, asciiSection[0].Length)
                .Select(colIndex => new string(asciiSection.Select(row => 
                    row[colIndex] == '-' ? '|' : (row[colIndex] == '|' ? '-' : row[colIndex])).ToArray()))
                .ToArray();
        }

        private static string[] MirrorSectionHorizontal(string[] asciiSection)
        {
            return asciiSection.Select(str => new string(str.Reverse().ToArray())).ToArray();
        }
        
        private static string[] MirrorSectionVertical(string[] asciiSection)
        {
            return asciiSection.Reverse().ToArray();
        }
        
        static string[] SwitchOnesAndTwos(string[] inputArray)
        {
            return inputArray.Select(line => new string(line.Select(c => c == '1' ? '2' : (c == '2' ? '1' : c)).ToArray())).ToArray();
        }
        
        #endregion

        private static string[] TransformSectionGraphics(SectionTypes sectionType, Direction currentDirection)
        {
            if (currentDirection == Direction.Right)
            {
                return AsciiSectionGraphicsDict[sectionType];
            }
            if (currentDirection == Direction.Left)
            {
                return MirrorSectionHorizontal(MirrorSectionVertical(AsciiSectionGraphicsDict[sectionType]));
            }
            
            if (sectionType == SectionTypes.RightCorner || sectionType == SectionTypes.LeftCorner)
            {
                var flipDirectionIsHorizontal = (Convert.ToInt32(sectionType == SectionTypes.RightCorner) + 
                                    Convert.ToInt32(currentDirection == Direction.Up)) != 1;   
                
                var directionCorrectedGraphics = flipDirectionIsHorizontal ? 
                    MirrorSectionHorizontal(AsciiSectionGraphicsDict[sectionType])
                    : MirrorSectionVertical(AsciiSectionGraphicsDict[sectionType]);

                return sectionType == SectionTypes.RightCorner ? SwitchOnesAndTwos(directionCorrectedGraphics)
                    : directionCorrectedGraphics;
            }
            
            return currentDirection == Direction.Up
                ? TransposeSectionVertical(MirrorSectionHorizontal(AsciiSectionGraphicsDict[sectionType]))
                : MirrorSectionHorizontal(TransposeSectionVertical(AsciiSectionGraphicsDict[sectionType]));
        }
        
        private enum Direction
        {
            Right = 0,
            Down = 1,
            Left = 2,
            Up = 3
        }
        
        private class SectionDetails
        {
            public SectionData Data { get; }
            public int XPos { get; }
            public int YPos { get; }
            public string[] AsciiSection { get; }

            public SectionDetails(SectionData data, int xPos, int yPos, string[] asciiSection)
            {
                Data = data;
                XPos = xPos;
                YPos = yPos;
                AsciiSection = asciiSection;
            }
        }
    }
}