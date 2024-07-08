using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using KOMPAS;
using Kompas6API5;
using Kompas6Constants;
using KompasAPI7;
using System.Threading.Tasks;

namespace gRPCLibrary.Services {

    public class KompasService : Kompas.KompasBase {

        public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context) {
            return Task.FromResult(new HelloResponse { Message = "Привет, " + request.Name });
        }

        public override Task<BoolValue> IsDocumentOpen(Empty request, ServerCallContext context) {
            return Task.FromResult(new BoolValue { Value = Program.Application.ActiveDocument != null });
        }

        public override Task<ObjectListResponse> GetObjects(ObjectListRequest request, ServerCallContext context) {
            ObjectListResponse response = new ObjectListResponse();
            IKompasDocument document = Program.Application.ActiveDocument;
            if (document == null || !(document is IKompasDocument2D)) {
                return Task.FromResult(response);
            }
            ksDocument2D doc2D = Program.KompasAPI.ActiveDocument2D();
            var dc = ((IKompasDocument2D) document).ViewsAndLayersManager.Views.ActiveView;
            if (!(dc is IDrawingContainer container) || container.get_Objects(0) == null) {
                return Task.FromResult(response);
            }
            foreach (IDrawingObject dObject in ((IDrawingContainer) dc).get_Objects(0)) {
                if (!request.Types_.Contains((int) dObject.DrawingObjectType)) {
                    continue;
                }
                var o = new ObjectListResponse.Types.Object {
                    Name = doc2D.ksGetObjectNameByType((int) dObject.DrawingObjectType),
                };
                switch (dObject.DrawingObjectType) {
                    case DrawingObjectTypeEnum.ksDrLDimension:
                        ILineDimension ld = (ILineDimension) dObject;
                        o.LineDimension = new ObjectListResponse.Types.Object.Types.LineDimension {
                            Angle = ld.Angle,
                            Orientation = (int) ld.Orientation,
                            ShelfX = ld.ShelfX, ShelfY = ld.ShelfY,
                            X1 = ld.X1, X2 = ld.X2, X3 = ld.X3,
                            Y1 = ld.Y1, Y2 = ld.Y2, Y3 = ld.Y3
                        };
                        break;
                    case DrawingObjectTypeEnum.ksDrADimension:
                        IAngleDimension ad = (IAngleDimension) dObject;
                        o.AngleDimension = new ObjectListResponse.Types.Object.Types.AngleDimension {
                            Angle1 = ad.Angle1,
                            Angle2 = ad.Angle2,
                            DimensionType = (int) ad.DimensionType,
                            Direction = ad.Direction,
                            Radius = ad.Radius,
                            ShelfX = ad.ShelfX, ShelfY = ad.ShelfY,
                            X1 = ad.X1, X2 = ad.X2, X3 = ad.X3, XC = ad.Xc,
                            Y1 = ad.Y1, Y2 = ad.Y2, Y3 = ad.Y3, YC = ad.Yc
                        };
                        break;
                    case DrawingObjectTypeEnum.ksDrDDimension:
                        IDiametralDimension dd = (IDiametralDimension) dObject;
                        o.DiametralDimension = new ObjectListResponse.Types.Object.Types.DiametralDimension {
                            Angle = dd.Angle,
                            DimensionType = dd.DimensionType,
                            Radius = dd.Radius,
                            XC = dd.Xc, YC = dd.Yc
                        };
                        break;
                    case DrawingObjectTypeEnum.ksDrRDimension:
                        IRadialDimension rd = (IRadialDimension) dObject;
                        o.RadialDimension = new ObjectListResponse.Types.Object.Types.RadialDimension {
                            Angle = rd.Angle,
                            BranchsCount = rd.BranchsCount,
                            DimensionType = rd.DimensionType,
                            Radius = rd.Radius,
                            ShelfX = rd.ShelfX, ShelfY = rd.ShelfY,
                            XC = rd.Xc, YC = rd.Yc
                        };
                        for (int i = 0; i < rd.BranchsCount; i++) {
                            o.RadialDimension.BranchBegin.Add(rd.BranchBegin[i]);
                        }
                        break;
                    case DrawingObjectTypeEnum.ksDrRBreakDimension:
                        IBreakRadialDimension brd = (IBreakRadialDimension) dObject;
                        o.BreakRadialDimension = new ObjectListResponse.Types.Object.Types.BreakRadialDimension {
                            Angle = brd.Angle,
                            BreakAngle = brd.BreakAngle,
                            BreakLength = brd.BreakLength,
                            BreakX1 = brd.BreakX1, BreakX2 = brd.BreakX2,
                            BreakY1 = brd.BreakY1, BreakY2 = brd.BreakY2,
                            Radius = brd.Radius,
                            TextOnLine = (int) brd.TextOnLine,
                            XC = brd.Xc, YC = brd.Yc
                        };
                        break;
                    case DrawingObjectTypeEnum.ksDrLBreakDimension:
                        IBreakLineDimension bld = (IBreakLineDimension) dObject;
                        o.BreakLineDimension = new ObjectListResponse.Types.Object.Types.BreakLineDimension {
                            ShelfX = bld.ShelfX, ShelfY = bld.ShelfY,
                            X1 = bld.X1, X2 = bld.X2, X3 = bld.X3,
                            Y1 = bld.Y1, Y2 = bld.Y2, Y3 = bld.Y3
                        };
                        break;
                    case DrawingObjectTypeEnum.ksDrABreakDimension:
                        IBreakAngleDimension bad = (IBreakAngleDimension) dObject;
                        o.BreakAngleDimension = new ObjectListResponse.Types.Object.Types.BreakAngleDimension {
                            Angle1 = bad.Angle1,
                            Angle2 = bad.Angle2,
                            DimensionType = (int) bad.DimensionType,
                            Direction = bad.Direction,
                            Radius = bad.Radius,
                            ShelfX = bad.ShelfX, ShelfY = bad.ShelfY,
                            X1 = bad.X1, X2 = bad.X2, X3 = bad.X3, XC = bad.Xc,
                            Y1 = bad.Y1, Y2 = bad.Y2, Y3 = bad.Y3, YC = bad.Yc
                        };
                        break;
                    case DrawingObjectTypeEnum.ksDrOrdinateDimension:
                        IHeightDimension hd = (IHeightDimension) dObject;
                        o.HeightDimension = new ObjectListResponse.Types.Object.Types.HeightDimension {
                            DimensionType = (int) hd.DimensionType,
                            X = hd.X, X1 = hd.X1, X2 = hd.X2,
                            Y = hd.X, Y1 = hd.Y1, Y2 = hd.Y2,
                        };
                        break;
                    case DrawingObjectTypeEnum.ksDrArcDimension:
                        IArcDimension arcD = (IArcDimension) dObject;
                        o.ArcDimension = new ObjectListResponse.Types.Object.Types.ArcDimension {
                            DimensionType = arcD.DimensionType,
                            Direction = arcD.Direction,
                            ShelfX = arcD.ShelfX, ShelfY = arcD.ShelfY,
                            TextPointer = arcD.TextPointer,
                            X1 = arcD.X1, X2 = arcD.X2, X3 = arcD.X3, XC = arcD.Xc,
                            Y1 = arcD.Y1, Y2 = arcD.Y2, Y3 = arcD.Y3, YC = arcD.Yc
                        };
                        break;
                    default:
                        break;
                }
                response.Objects.Add(o);
            }
            return Task.FromResult(response);
        }
    }
}
