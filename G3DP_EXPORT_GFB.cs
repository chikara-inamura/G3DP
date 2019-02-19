using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;



/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public class Script_Instance : GH_ScriptInstance
{
#region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { /* Implementation hidden. */ }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { /* Implementation hidden. */ }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { /* Implementation hidden. */ }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
#endregion

#region Members
  /// <summary>Gets the current Rhino document.</summary>
  private readonly RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private readonly GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private readonly IGH_Component Component;
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private readonly int Iteration;
#endregion

  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments,
  /// Output parameters as ref arguments. You don't have to assign output parameters,
  /// they will have a default value.
  /// </summary>
  private void RunScript(Brep B, double H, int D, Point3d P, double F, ref object X, ref object S, ref object T, ref object G)
  {
    double DT = 0.01;
    Vector3d VX = new Vector3d(1, 0, 0);
    Vector3d VY = new Vector3d(0, 1, 0);

    BoundingBox BX = B.GetBoundingBox(true);
    Point3d P0 = BX.PointAt(0.5, 0.5, 0.0);
    Point3d P1 = BX.PointAt(0.5, 0.5, 1.0);
    double DZ = P1.Z - P0.Z;

    int i = 0;
    int IC = (int) (DZ / H);
    List<Curve> LSC = new List<Curve>();
    List<Point3d[]> LSPD = new List<Point3d[]>();
    for(i = 0; i <= IC; i++)
    {
      Point3d PT = new Point3d(0, 0, H * i);
      Plane PL = new Rhino.Geometry.Plane(PT, VX, VY);
      Curve[] CI;
      Point3d[] PI;
      bool BI = Rhino.Geometry.Intersect.Intersection.BrepPlane(B, PL, DT, out CI, out PI);
      Curve CX = CI[0];
      if(i > 0)
      {
        Curve CX0 = LSC[0];
        bool BC = Curve.DoDirectionsMatch(CX0, CX);
        if (BC == false)
        {
          bool BR = CX.Reverse();
        }
      }
      LSC.Add(CX);
    }

    int j = 0;
    List<Point3d> LSPJ = new List<Point3d>();
    for(i = 0; i <= IC - 1; i++)
    {
      Curve C0 = LSC[i];
      Curve C1 = LSC[i + 1];
      Point3d[] PD0, PD1;
      double[] DD0, DD1;
      DD0 = C0.DivideByCount(D, true, out PD0);
      DD1 = C1.DivideByCount(D, true, out PD1);
      for(j = 0; j < D; j++)
      {
        Point3d PDJ0 = PD0[j];
        Point3d PDJ1 = PD1[j];
        double DJ = j / (double) D;
        Point3d PJ = PDJ0 + DJ * (PDJ1 - PDJ0);
        LSPJ.Add(PJ);
      }
    }
    Polyline PL0 = new Polyline(LSPJ);

    List<Point3d> LSPT = new List<Point3d>();
    List<string> LSSG = new List<string>();
    Point3d PC = P;

    int k = 0;
    for(k = 0; k < LSPJ.Count; k++)
    {
      Point3d PK = LSPJ[k];
      Point3d PT = new Point3d(PC.X + PK.X, PC.Y + PK.Y, PC.Z - PK.Z);
      LSPT.Add(PT);
      double DPX = Math.Round(PT.X, 2);
      double DPY = Math.Round(PT.Y, 2);
      double DPZ = Math.Round(PT.Z, 2);
      double DF = Math.Round(F, 2);
      string SG = "G1 " + "F" + DF + " X" + DPX + " Y" + DPY + " Z" + DPZ;
      LSSG.Add(SG);
    }

    X = LSPJ;
    S = PL0;
    T = LSPT;
    G = LSSG;

  }

  // <Custom additional code> 

  // </Custom additional code> 
}