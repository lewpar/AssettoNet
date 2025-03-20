using System.IO;
using System.Threading.Tasks;
using System;

using AssettoNet.IO;

namespace AssettoNet.Network
{
    public class AssettoUpdateResponse
    {
        public char Identifier { get; set; }
        public int Size { get; set; }

        public float SpeedKmh { get; set; }
        public float SpeedMph { get; set; }
        public float SpeedMs { get; set; }

        public bool IsAbsEnabled { get; set; }
        public bool IsAbsInAction { get; set; }
        public bool IsTcInAction { get; set; }
        public bool IsTcEnabled { get; set; }
        public bool IsInPit { get; set; }
        public bool IsEngineLimiterOn { get; set; }

        public float AccGVertical { get; set; }
        public float AccGHorizontal { get; set; }
        public float AccGFrontal { get; set; }

        public TimeSpan LapTime { get; set; }
        public TimeSpan LastLap { get; set; }
        public TimeSpan BestLap { get; set; }
        public int LapCount { get; set; }

        public float Gas { get; set; }
        public float Brake { get; set; }
        public float Clutch { get; set; }
        public float EngineRPM { get; set; }
        public float Steer { get; set; }
        public int Gear { get; set; }
        public float CgHeight { get; set; }

        public float[] WheelAngularSpeed { get; set; }
        public float[] SlipAngle { get; set; }
        public float[] SlipAngleContactPatch { get; set; }
        public float[] SlipRatio { get; set; }
        public float[] TyreSlip { get; set; }
        public float[] NdSlip { get; set; }
        public float[] Load { get; set; }
        public float[] Dy { get; set; }
        public float[] Mz { get; set; }
        public float[] TyreDirtyLevel { get; set; }
        public float[] CamberRad { get; set; }
        public float[] TyreRadius { get; set; }
        public float[] TyreLoadedRadius { get; set; }
        public float[] SuspensionHeight { get; set; }

        public float CarPositionNormalized { get; set; }
        public float CarSlope { get; set; }

        public float[] CarCoordinates { get; set; }

        public AssettoUpdateResponse()
        {
            Identifier = char.MinValue;
            Size = 0;

            SpeedKmh = 0;
            SpeedMph = 0;
            SpeedMs = 0;

            IsAbsEnabled = false;
            IsAbsInAction = false;
            IsTcEnabled = false;
            IsTcInAction = false;
            IsInPit = false;
            IsEngineLimiterOn = false;

            AccGVertical = 0f;
            AccGHorizontal = 0f;
            AccGFrontal = 0f;

            LapTime = TimeSpan.Zero;
            LastLap = TimeSpan.Zero;
            BestLap = TimeSpan.Zero;
            LapCount = 0;

            Gas = 0f;
            Brake = 0f;
            Clutch = 0f;
            EngineRPM = 0f;
            Steer = 0f;
            Gear = 0;
            CgHeight = 0f;

            WheelAngularSpeed = new float[4];
            SlipAngle = new float[4];
            SlipAngleContactPatch = new float[4];
            SlipRatio = new float[4];
            TyreSlip = new float[4];
            NdSlip = new float[4];
            Load = new float[4];
            Dy = new float[4];
            Mz = new float[4];
            TyreDirtyLevel = new float[4];
            CamberRad = new float[4];
            TyreRadius = new float[4];
            TyreLoadedRadius = new float[4];
            SuspensionHeight = new float[4];

            CarPositionNormalized = 0f;
            CarSlope = 0f;

            CarCoordinates = new float[4];
        }

        public async static Task<AssettoUpdateResponse> DeserializeAsync(byte[] data)
        {
            using var stream = new MemoryStream(data);

            var identifier = await stream.ReadCharAsync();

            // memory alignment
            _ = await stream.ReadByteAsync();
            _ = await stream.ReadByteAsync();
            _ = await stream.ReadByteAsync();

            var size = await stream.ReadIntAsync();

            var speedKmh = await stream.ReadFloatAsync();

            var speedMph = await stream.ReadFloatAsync();
            var speedMs = await stream.ReadFloatAsync();

            var isAbsEnabled = await stream.ReadBooleanAsync();
            var isAbsInAction = await stream.ReadBooleanAsync();
            var isTcInAction = await stream.ReadBooleanAsync();
            var isTcEnabled = await stream.ReadBooleanAsync();
            var isInPit = await stream.ReadBooleanAsync();
            var isEngineLimiterOn = await stream.ReadBooleanAsync();

            // memory alignment
            _ = await stream.ReadByteAsync();
            _ = await stream.ReadByteAsync();

            var accGVertical = await stream.ReadFloatAsync();
            var accGHorizontal = await stream.ReadFloatAsync();
            var accGFrontal = await stream.ReadFloatAsync();

            var lapTime = TimeSpan.FromMilliseconds(await stream.ReadIntAsync());
            var lastLap = TimeSpan.FromMilliseconds(await stream.ReadIntAsync());
            var bestLap = TimeSpan.FromMilliseconds(await stream.ReadIntAsync());
            var lapCount = await stream.ReadIntAsync();

            var gas = await stream.ReadFloatAsync();
            var brake = await stream.ReadFloatAsync();
            var clutch = await stream.ReadFloatAsync();
            var engineRPM = await stream.ReadFloatAsync();
            var steer = await stream.ReadFloatAsync();
            var gear = await stream.ReadIntAsync();
            var cgHeight = await stream.ReadFloatAsync();

            var wheelAngularSpeed = await stream.ReadFloatArrayAsync(4);
            var slipAngle = await stream.ReadFloatArrayAsync(4);
            var slipAngleContactPatch = await stream.ReadFloatArrayAsync(4);
            var slipRatio = await stream.ReadFloatArrayAsync(4);
            var tyreSlip = await stream.ReadFloatArrayAsync(4);
            var ndSlip = await stream.ReadFloatArrayAsync(4);
            var load = await stream.ReadFloatArrayAsync(4);
            var dy = await stream.ReadFloatArrayAsync(4);
            var mz = await stream.ReadFloatArrayAsync(4);
            var tyreDirtyLevel = await stream.ReadFloatArrayAsync(4);
            var camberRad = await stream.ReadFloatArrayAsync(4);
            var tyreRadius = await stream.ReadFloatArrayAsync(4);
            var tyreLoadedRadius = await stream.ReadFloatArrayAsync(4);
            var suspensionHeight = await stream.ReadFloatArrayAsync(4);

            var carPositionNormalized = await stream.ReadFloatAsync();
            var carSlope = await stream.ReadFloatAsync();

            var carCoordinates = await stream.ReadFloatArrayAsync(4);

            return new AssettoUpdateResponse()
            {
                Identifier = (char)identifier,
                Size = size,
                SpeedKmh = speedKmh,
                SpeedMph = speedMph,
                SpeedMs = speedMs,
                IsAbsEnabled = isAbsEnabled,
                IsAbsInAction = isAbsInAction,
                IsTcInAction = isTcInAction,
                IsTcEnabled = isTcEnabled,
                IsInPit = isInPit,
                IsEngineLimiterOn = isEngineLimiterOn,
                AccGVertical = accGVertical,
                AccGHorizontal = accGHorizontal,
                AccGFrontal = accGFrontal,
                LapTime = lapTime,
                LastLap = lastLap,
                BestLap = bestLap,
                LapCount = lapCount,
                Gas = gas,
                Brake = brake,
                Clutch = clutch,
                EngineRPM = engineRPM,
                Steer = steer,
                Gear = gear,
                CgHeight = cgHeight,
                WheelAngularSpeed = wheelAngularSpeed,
                SlipAngle = slipAngle,
                SlipAngleContactPatch = slipAngleContactPatch,
                SlipRatio = slipRatio,
                TyreSlip = tyreSlip,
                NdSlip = ndSlip,
                Load = load,
                Dy = dy,
                Mz = mz,
                TyreDirtyLevel = tyreDirtyLevel,
                CamberRad = camberRad,
                TyreRadius = tyreRadius,
                TyreLoadedRadius = tyreLoadedRadius,
                SuspensionHeight = suspensionHeight,
                CarPositionNormalized = carPositionNormalized,
                CarSlope = carSlope,
                CarCoordinates = carCoordinates
            };
        }

        public override string ToString()
        {
            return $"Identifier: {Identifier} " +
                $"Size: {Size} bytes" +
                $"Speed: {SpeedKmh} km/h, {SpeedMph} mph, {SpeedMs} m/s" +
                $"ABS: Enabled={IsAbsEnabled}, In Action={IsAbsInAction}" +
                $"TC: Enabled={IsTcEnabled}, In Action={IsTcInAction}" +
                $"Pit Status: {IsInPit}, Engine Limiter: {IsEngineLimiterOn}" +
                $"Acceleration: Vertical={AccGVertical}, Horizontal={AccGHorizontal}, Frontal={AccGFrontal}" +
                $"Lap Times: Current={LapTime}, Last={LastLap}, Best={BestLap}, Count={LapCount}" +
                $"Controls: Gas={Gas:P1}, Brake={Brake:P1}, Clutch={Clutch:P1}, Steer={Steer:F2}, Gear={Gear}, RPM={EngineRPM}" +
                $"CG Height: {CgHeight:F2}" +
                $"Wheel Speeds: {string.Join(", ", WheelAngularSpeed)}" +
                $"Slip Angles: {string.Join(", ", SlipAngle)}" +
                $"Slip Contact: {string.Join(", ", SlipAngleContactPatch)}" +
                $"Slip Ratio: {string.Join(", ", SlipRatio)}" +
                $"Tyre Slip: {string.Join(", ", TyreSlip)}" +
                $"ND Slip: {string.Join(", ", NdSlip)}" +
                $"Load: {string.Join(", ", Load)}" +
                $"Dy: {string.Join(", ", Dy)}" +
                $"Mz: {string.Join(", ", Mz)}" +
                $"Tyre Dirt: {string.Join(", ", TyreDirtyLevel)}" +
                $"Camber Rad: {string.Join(", ", CamberRad)}" +
                $"Tyre Radius: {string.Join(", ", TyreRadius)}" +
                $"Loaded Radius: {string.Join(", ", TyreLoadedRadius)}" +
                $"Suspension Height: {string.Join(", ", SuspensionHeight)}" +
                $"Car Position: {CarPositionNormalized:F2}, Slope: {CarSlope:F2}" +
                $"Coordinates: {string.Join(", ", CarCoordinates)}";
        }
    }
}
