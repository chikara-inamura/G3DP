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
  private void RunScript(List<Curve> C, int D, Point3d P, double F, ref object X, ref object S, ref object T, ref object G)
  {

    Vector3d VX = new Vector3d(1, 0, 0);
    Vector3d VY = new Vector3d(0, 1, 0);

    List<double> LSDZ = new List<double>();
    int i = 0;
    int IC = C.Count;
    for(i = 0; i < C.Count; i++)
    {
      Curve C0 = C[i];
      BoundingBox BC0 = C0.GetBoundingBox(true);
      Point3d PC0 = BC0.Center;
      double DZ0 = PC0.Z;
      LSDZ.Add(DZ0);
    }
    double[] ADZ = LSDZ.ToArray();
    Curve[] AC = C.ToArray();
    Array.Sort(ADZ, AC);
    List<Curve> LSC = new List<Curve>(AC);

    for(i = 0; i < IC; i++)
    {
      Curve C0 = LSC[0];
      Curve C1 = LSC[i];
      if(i > 0)
      {
        bool BC = Curve.DoDirectionsMatch(C0, C1);
        if (BC == false)
        {
          bool BR = C1.Reverse();
        }
      }
      LSC[i] = C1;
    }

    int j = 0;
    List<Point3d> LSPJ = new List<Point3d>();
    for(i = 0; i < IC - 1; i++)
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