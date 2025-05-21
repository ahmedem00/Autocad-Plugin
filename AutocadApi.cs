using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;



/* ----------------- -- -- -- ---
                    */
namespace Demo00
{

    public class AutocadApi
    {
        [CommandMethod("CreateDimension")]
        public void CreateDimension()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptPointOptions pointOptions = new PromptPointOptions("\nPick a base point for the grid:");
            PromptPointResult pointResult = ed.GetPoint(pointOptions);

            if (pointResult.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nNo point selected. Command cancelled.");
                return;
            }

            Point3d basePoint = pointResult.Value;
            if (!int.TryParse(Environment.GetEnvironmentVariable("GRID_X_COUNT"), out int xCount)) ;
            if (!int.TryParse(Environment.GetEnvironmentVariable("GRID_Y_COUNT"), out int yCount)) ;
            if (!float.TryParse(Environment.GetEnvironmentVariable("GRID_SPACING"), out float spacing)) ;

            ed.WriteMessage($"\nCreateDimension command executed with xCount={xCount}, yCount={yCount}, spacing={spacing}");

            List<Line> linesX = new List<Line>();
            List<Line> linesY = new List<Line>();
            for (int i = 0; i < xCount; i++)// 10>>> number of grids   0..1..2..3..4..5..6..7..8..9
            {
                Point3d point0 = new Point3d(-2, 0 + (i * spacing), 0); ////   lsdt at -3,90,0
                Point3d point1 = new Point3d((yCount * spacing), 0 + (i * spacing), 0);////last at 100,90,0
                Line line = new Line(point0, point1);
                linesX.Add(line);
                //double y = basePoint.Y + (i * spacing);
                //Point3d startX = new Point3d(basePoint.X - 2, y, basePoint.Z);
                //Point3d endX = new Point3d(basePoint.X + (yCount * spacing), y, basePoint.Z);
                //linesX.Add(new Line(startX, endX));
            }
            for (int i = 0; i < yCount; i++)
            {
                Point3d point0 = new Point3d(0 + (i * spacing), -2, 0);
                Point3d point1 = new Point3d(0 + (i * spacing), (xCount * spacing), 0);
                Line line = new Line(point0, point1);
                linesY.Add(line);
                //double x = basePoint.X + (i * spacing);

                //Point3d startY = new Point3d(x, basePoint.Y - 2, basePoint.Z);
                //Point3d endY = new Point3d(x, basePoint.Y + (xCount * spacing), basePoint.Z);
                //linesY.Add(new Line(startY, endY));
            }




            using (Transaction acTransc = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = acTransc.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord record = acTransc.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;


                LayerTable layerTable = acTransc.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;

                string newLayerName = "L1";
                string linetypeName = "CENTER";

                if (!layerTable.Has(newLayerName))
                {
                    layerTable.UpgradeOpen();

                    LinetypeTable ltypeTable = acTransc.GetObject(db.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                    if (!ltypeTable.Has(linetypeName))
                    {
                        db.LoadLineTypeFile(linetypeName, "acad.lin");
                    }

                    LayerTableRecord newLayer = new LayerTableRecord
                    {
                        Name = newLayerName,
                        Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 0),
                        LinetypeObjectId = ltypeTable[linetypeName]
                    };

                    layerTable.Add(newLayer);
                    acTransc.AddNewlyCreatedDBObject(newLayer, true);
                    ed.WriteMessage($"\nLayer '{newLayerName}' created with '{linetypeName}' linetype.");
                }
                //ObjectId gridlayer = layerTable["0"];
                foreach (Line line in linesX)
                {
                    if (layerTable.Has("L1"))
                    {
                        line.Layer = "L1";
                        record.AppendEntity(line);
                        acTransc.AddNewlyCreatedDBObject(line, true);
                    }
                }


                foreach (Line line in linesY)
                {
                    line.Layer = "L1";
                    record.AppendEntity(line);
                    acTransc.AddNewlyCreatedDBObject(line, true);

                    for (int i = 0; i < xCount - 1; i++)
                    {
                        using (Dimension dimension = new AlignedDimension(new Point3d(0, 0 + (i * spacing), 0), new Point3d(0, spacing + (i * spacing), 0), new Point3d(-5, 0, 0), string.Empty, db.Dimstyle)) // for grid X axis
                        {
                            record.AppendEntity(dimension);
                            acTransc.AddNewlyCreatedDBObject(dimension, true);

                        }
                        using (Dimension dimension = new AlignedDimension(new Point3d(0, 0, 0), new Point3d(0, (xCount - 1) * spacing, 0), new Point3d(-10, 0, 0), string.Empty, db.Dimstyle)) // for grid X axis
                        {
                            record.AppendEntity(dimension);
                            acTransc.AddNewlyCreatedDBObject(dimension, true);

                        }
                    }
                }


               

                for (int i = 0; i < yCount - 1; i++)
                {
                    using (Dimension dimension = new AlignedDimension(new Point3d(0 + (i * spacing), 0, 0), new Point3d(spacing + (i * spacing), 0, 0), new Point3d(0, -10, 0), string.Empty, db.Dimstyle)) // for grid Y axis
                    {
                        record.AppendEntity(dimension);
                        acTransc.AddNewlyCreatedDBObject(dimension, true);

                    }
                    using (Dimension dimension = new AlignedDimension(new Point3d(0, 0, 0), new Point3d((yCount - 1) * spacing, 0, 0), new Point3d(0, -5, 0), string.Empty, db.Dimstyle)) // for grid Y axis
                    {
                        record.AppendEntity(dimension);
                        acTransc.AddNewlyCreatedDBObject(dimension, true);

                    }

                }




                
                acTransc.Commit();
            }




        }




    }
}

