using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using KOMPAS;
using Kompas6Constants;
using System.Text;
using static KOMPAS.ObjectListResponse.Types.Object;

namespace WebClient.Clients {

    public class GrpcClient {

        private readonly Kompas.KompasClient _client;

        public GrpcClient(string address) {
            var channel = GrpcChannel.ForAddress(address);
            _client = new Kompas.KompasClient(channel);
        }

        public string SayHello() {
            return "";
        }

        public string IsDocumentOpen() {
            var isDocumentOpen = _client.IsDocumentOpen(new Empty());
            if (isDocumentOpen != null) {
                return isDocumentOpen.Value ? "Все готово к работе" : "Не открыт документ";
            }
            return "Не запущен КОМПАС-3D";
        }

        public string GetObjects() {
            var request = new ObjectListRequest();
            request.Types_.AddRange(new List<int> {
                (int) DrawingObjectTypeEnum.ksDrLDimension,
                (int) DrawingObjectTypeEnum.ksDrADimension,
                (int) DrawingObjectTypeEnum.ksDrDDimension,
                (int) DrawingObjectTypeEnum.ksDrRDimension,
                (int) DrawingObjectTypeEnum.ksDrRBreakDimension,
                (int) DrawingObjectTypeEnum.ksDrLBreakDimension,
                (int) DrawingObjectTypeEnum.ksDrABreakDimension,
                (int) DrawingObjectTypeEnum.ksDrOrdinateDimension,
                (int) DrawingObjectTypeEnum.ksDrArcDimension
            });

            StringBuilder sb = new StringBuilder();
            var objects = _client.GetObjects(request);
            if (objects != null) {
                foreach (ObjectListResponse.Types.Object dObject in objects.Objects) {
                    sb.AppendLine("============================");
                    sb.AppendLine($"Name = {dObject.Name}");
                    switch (dObject.DataCase) {
                        case DataOneofCase.LineDimension:
                            var lineDim = dObject.LineDimension;
                            sb.AppendLine($"Angle = {lineDim.Angle}");
                            sb.AppendLine($"Orientation = {lineDim.Orientation}");
                            sb.AppendLine($"ShelfX = {lineDim.ShelfX}");
                            sb.AppendLine($"ShelfY = {lineDim.ShelfY}");
                            sb.AppendLine($"X1 = {lineDim.X1}");
                            sb.AppendLine($"X2 = {lineDim.X2}");
                            sb.AppendLine($"X3 = {lineDim.X3}");
                            sb.AppendLine($"Y1 = {lineDim.Y1}");
                            sb.AppendLine($"Y2 = {lineDim.Y2}");
                            sb.AppendLine($"Y3 = {lineDim.Y3}");
                            break;
                        case DataOneofCase.AngleDimension:
                            var angleDim = dObject.AngleDimension;
                            sb.AppendLine($"Angle1 = {angleDim.Angle1}");
                            sb.AppendLine($"Angle2 = {angleDim.Angle2}");
                            sb.AppendLine($"DimensionType = {angleDim.DimensionType}");
                            sb.AppendLine($"Direction = {angleDim.Direction}");
                            sb.AppendLine($"Radius = {angleDim.Radius}");
                            sb.AppendLine($"ShelfX = {angleDim.ShelfX}");
                            sb.AppendLine($"ShelfY = {angleDim.ShelfY}");
                            sb.AppendLine($"XC = {angleDim.XC}");
                            sb.AppendLine($"X1 = {angleDim.X1}");
                            sb.AppendLine($"X2 = {angleDim.X2}");
                            sb.AppendLine($"X3 = {angleDim.X3}");
                            sb.AppendLine($"YC = {angleDim.YC}");
                            sb.AppendLine($"Y1 = {angleDim.Y1}");
                            sb.AppendLine($"Y2 = {angleDim.Y2}");
                            sb.AppendLine($"Y3 = {angleDim.Y3}");
                            break;
                        case DataOneofCase.DiametralDimension:
                            var diametralDim = dObject.DiametralDimension;
                            sb.AppendLine($"Angle = {diametralDim.Angle}");
                            sb.AppendLine($"DimensionType = {diametralDim.DimensionType}");
                            sb.AppendLine($"Radius = {diametralDim.Radius}");
                            sb.AppendLine($"XC = {diametralDim.XC}");
                            sb.AppendLine($"YC = {diametralDim.YC}");
                            break;
                        case DataOneofCase.RadialDimension:
                            var radialDim = dObject.RadialDimension;
                            sb.AppendLine($"Angle = {radialDim.Angle}");
                            sb.AppendLine($"BranchsCount = {radialDim.BranchsCount}");
                            sb.AppendLine($"DimensionType = {radialDim.DimensionType}");
                            sb.AppendLine($"Radius = {radialDim.Radius}");
                            sb.AppendLine($"ShelfX = {radialDim.ShelfX}");
                            sb.AppendLine($"ShelfY = {radialDim.ShelfY}");
                            sb.AppendLine($"XC = {radialDim.XC}");
                            sb.AppendLine($"YC = {radialDim.YC}");
                            break;
                        case DataOneofCase.BreakRadialDimension:
                            var breakRadialDim = dObject.BreakRadialDimension;
                            sb.AppendLine($"Angle = {breakRadialDim.Angle}");
                            sb.AppendLine($"BreakAngle = {breakRadialDim.BreakAngle}");
                            sb.AppendLine($"BreakLength = {breakRadialDim.BreakLength}");
                            sb.AppendLine($"BreakX1 = {breakRadialDim.BreakX1}");
                            sb.AppendLine($"BreakX2 = {breakRadialDim.BreakX2}");
                            sb.AppendLine($"BreakY1 = {breakRadialDim.BreakY1}");
                            sb.AppendLine($"BreakY2 = {breakRadialDim.BreakY2}");
                            sb.AppendLine($"Radius = {breakRadialDim.Radius}");
                            sb.AppendLine($"TextOnLine = {breakRadialDim.TextOnLine}");
                            sb.AppendLine($"XC = {breakRadialDim.XC}");
                            sb.AppendLine($"YC = {breakRadialDim.YC}");
                            break;
                        case DataOneofCase.BreakLineDimension:
                            var breakLineDim = dObject.BreakLineDimension;
                            sb.AppendLine($"ShelfX = {breakLineDim.ShelfX}");
                            sb.AppendLine($"ShelfY = {breakLineDim.ShelfY}");
                            sb.AppendLine($"X1 = {breakLineDim.X1}");
                            sb.AppendLine($"X2 = {breakLineDim.X2}");
                            sb.AppendLine($"X3 = {breakLineDim.X3}");
                            sb.AppendLine($"Y1 = {breakLineDim.Y1}");
                            sb.AppendLine($"Y2 = {breakLineDim.Y2}");
                            sb.AppendLine($"Y3 = {breakLineDim.Y3}");
                            break;
                        case DataOneofCase.BreakAngleDimension:
                            var breakAngleDim = dObject.BreakAngleDimension;
                            sb.AppendLine($"Angle1 = {breakAngleDim.Angle1}");
                            sb.AppendLine($"Angle2 = {breakAngleDim.Angle2}");
                            sb.AppendLine($"DimensionType = {breakAngleDim.DimensionType}");
                            sb.AppendLine($"Direction = {breakAngleDim.Direction}");
                            sb.AppendLine($"Radius = {breakAngleDim.Radius}");
                            sb.AppendLine($"ShelfX = {breakAngleDim.ShelfX}");
                            sb.AppendLine($"ShelfY = {breakAngleDim.ShelfY}");
                            sb.AppendLine($"XC = {breakAngleDim.XC}");
                            sb.AppendLine($"X1 = {breakAngleDim.X1}");
                            sb.AppendLine($"X2 = {breakAngleDim.X2}");
                            sb.AppendLine($"X3 = {breakAngleDim.X3}");
                            sb.AppendLine($"YC = {breakAngleDim.YC}");
                            sb.AppendLine($"Y1 = {breakAngleDim.Y1}");
                            sb.AppendLine($"Y2 = {breakAngleDim.Y2}");
                            sb.AppendLine($"Y3 = {breakAngleDim.Y3}");
                            break;
                        case DataOneofCase.HeightDimension:
                            var heightDim = dObject.HeightDimension;
                            sb.AppendLine($"DimensionType = {heightDim.DimensionType}");
                            sb.AppendLine($"X = {heightDim.X}");
                            sb.AppendLine($"X1 = {heightDim.X1}");
                            sb.AppendLine($"X2 = {heightDim.X2}");
                            sb.AppendLine($"Y = {heightDim.Y}");
                            sb.AppendLine($"Y1 = {heightDim.Y1}");
                            sb.AppendLine($"Y2 = {heightDim.Y2}");
                            break;
                        case DataOneofCase.ArcDimension:
                            var arcDim = dObject.ArcDimension;
                            sb.AppendLine($"DimensionType = {arcDim.DimensionType}");
                            sb.AppendLine($"Direction = {arcDim.Direction}");
                            sb.AppendLine($"ShelfX = {arcDim.ShelfX}");
                            sb.AppendLine($"ShelfY = {arcDim.ShelfY}");
                            sb.AppendLine($"TextPointer = {arcDim.TextPointer}");
                            sb.AppendLine($"XC = {arcDim.XC}");
                            sb.AppendLine($"X1 = {arcDim.X1}");
                            sb.AppendLine($"X2 = {arcDim.X2}");
                            sb.AppendLine($"X3 = {arcDim.X3}");
                            sb.AppendLine($"YC = {arcDim.YC}");
                            sb.AppendLine($"Y1 = {arcDim.Y1}");
                            sb.AppendLine($"Y2 = {arcDim.Y2}");
                            sb.AppendLine($"Y3 = {arcDim.Y3}");
                            break;
                    }
                    sb.AppendLine("============================");
                }
            }
            return sb.ToString();
        }
    }
}
