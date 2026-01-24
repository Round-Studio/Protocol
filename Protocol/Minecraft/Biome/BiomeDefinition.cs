using System;
using System.Collections.Generic; 
using System.Numerics;
namespace Protocol.Minecraft 
{
    public enum BiomeExpressionOp
    {
        Unknown = -1,
        LeftBrace = 0,
        RightBrace = 1,
        LeftBracket = 2,
        RightBracket = 3,
        LeftParenthesis = 4,
        RightParenthesis = 5,
        Negate = 6,
        LogicalNot = 7,
        Abs = 8,
        Add = 9,
        Acos = 10,
        Asin = 11,
        Atan = 12,
        Atan2 = 13,
        Ceil = 14,
        Clamp = 15,
        CopySign = 16,
        Cos = 17,
        DieRoll = 18,
        DieRollInt = 19,
        Div = 20,
        Exp = 21,
        Floor = 22,
        HermiteBlend = 23,
        Lerp = 24,
        LerpRotate = 25,
        Ln = 26,
        Max = 27,
        Min = 28,
        MinAngle = 29,
        Mod = 30,
        Mul = 31,
        Pow = 32,
        Random = 33,
        RandomInt = 34,
        Round = 35,
        Sin = 36,
        Sign = 37,
        Sqrt = 38,
        Trunc = 39,
        QueryFunction = 40,
        ArrayVariable = 41,
        ContextVariable = 42,
        EntityVariable = 43,
        TempVariable = 44,
        MemberAccessor = 45,
        HashedStringHash = 46,
        GeometryVariable = 47,
        MaterialVariable = 48,
        TextureVariable = 49,
        LessThan = 50,
        LessEqual = 51,
        GreaterEqual = 52,
        GreaterThan = 53,
        LogicalEqual = 54,
        LogicalNotEqual = 55,
        LogicalOr = 56,
        LogicalAnd = 57,
        NullCoalescing = 58,
        Conditional = 59,
        ConditionalElse = 60,
        Float = 61,
        Pi = 62,
        Array = 63,
        Geometry = 64,
        Material = 65,
        Texture = 66,
        Loop = 67,
        ForEach = 68,
        Break = 69,
        Continue = 70,
        Assignment = 71,
        Pointer = 72,
        Semicolon = 73,
        Return = 74,
        Comma = 75,
        This = 76,
    }

    public enum BiomeCoordinateEvaluationOrder
    {
        XYZ = 0,
        XZY = 1,
        YXZ = 2,
        YZX = 3,
        ZXY = 4,
        ZYX = 5,
    }

    public enum BiomeRandomDistributionType
    {
        SingleValued = 0,
        Uniform = 1,
        Gaussian = 2,
        InverseGaussian = 3,
        FixedGrid = 4,
        JitteredGrid = 5,
        Triangle = 6,
    }
    public struct BiomeReplacementData
    {
        
        
        
        public short Biome { get; set; }

        
        
        
        public short Dimension { get; set; }

        
        
        
        public short[] TargetBiomes { get; set; }

        
        
        
        public float Amount { get; set; }

        
        
        
        public float NoiseFrequencyScale { get; set; }

        
        
        
        public uint ReplacementIndex { get; set; }
    }

	

	
	public struct BiomeWeight
    {
        public short Biome { get; set; } 
        public uint Weight { get; set; } 
    }

    
    public struct BiomeTemperatureWeight
    {
        public int Temperature { get; set; } 
        public uint Weight { get; set; } 
    }

    
    public struct BiomeConditionalTransformation
    {
        public BiomeWeight[] WeightedBiomes { get; set; } 
        public short ConditionJSON { get; set; } 
        public uint MinPassingNeighbours { get; set; } 
    }

    
    public struct BiomeMultiNoiseRules
    {
        public float Temperature { get; set; } 
        public float Humidity { get; set; } 
        public float Altitude { get; set; } 
        public float Weirdness { get; set; } 
        public float Weight { get; set; } 
    }

    
    public struct BiomeOverworldRules
    {
        public BiomeWeight[] HillsTransformations { get; set; }
        public BiomeWeight[] MutateTransformations { get; set; }
        public BiomeWeight[] RiverTransformations { get; set; }
        public BiomeWeight[] ShoreTransformations { get; set; }
        public BiomeConditionalTransformation[] PreHillsEdgeTransformations { get; set; }
        public BiomeConditionalTransformation[] PostShoreEdgeTransformations { get; set; }
        public BiomeTemperatureWeight[] ClimateTransformations { get; set; }
    }

    
    public struct BiomeCappedSurface
    {
        public int[] FloorBlocks { get; set; } 
        public int[] CeilingBlocks { get; set; } 
        public Optional<uint> SeaBlock { get; set; } 
        public Optional<uint> FoundationBlock { get; set; } 
        public Optional<uint> BeachBlock { get; set; } 
    }

    
    public struct BiomeMesaSurface
    {
        public uint ClayMaterial { get; set; } 
        public uint HardClayMaterial { get; set; } 
        public bool BrycePillars { get; set; } 
        public bool HasForest { get; set; } 
    }

    
    public struct BiomeSurfaceMaterial
    {
        public int TopBlock { get; set; } 
        public int MidBlock { get; set; } 
        public int SeaFloorBlock { get; set; } 
        public int FoundationBlock { get; set; } 
        public int SeaBlock { get; set; } 
        public int SeaFloorDepth { get; set; } 
    }

    
    public struct BiomeElementData
    {
        public float NoiseFrequencyScale { get; set; } 
        public float NoiseLowerBound { get; set; } 
        public float NoiseUpperBound { get; set; } 
        public int HeightMinType { get; set; } 
        public short HeightMin { get; set; } 
        public int HeightMaxType { get; set; } 
        public short HeightMax { get; set; } 
        public BiomeSurfaceMaterial AdjustedMaterials { get; set; } 
    }

    
    public struct BiomeMountainParameters
    {
        public int SteepBlock { get; set; } 
        public bool NorthSlopes { get; set; } 
        public bool SouthSlopes { get; set; } 
        public bool WestSlopes { get; set; } 
        public bool EastSlopes { get; set; } 
        public bool TopSlideEnabled { get; set; } 
    }

    
    public struct BiomeCoordinate
    {
        public int MinValueType { get; set; } 
        public short MinValue { get; set; } 
        public int MaxValueType { get; set; } 
        public short MaxValue { get; set; } 
        public uint GridOffset { get; set; } 
        public uint GridStepSize { get; set; } 
        public int Distribution { get; set; } 
    }

    
    public struct BiomeScatterParameter
    {
        public BiomeCoordinate[] Coordinates { get; set; } 
        public int EvaluationOrder { get; set; } 
        public int ChancePercentType { get; set; } 
        public short ChancePercent { get; set; } 
        public int ChanceNumerator { get; set; } 
        public int ChanceDenominator { get; set; } 
        public int IterationsType { get; set; } 
        public short Iterations { get; set; } 
    }

    
    public struct BiomeConsolidatedFeature
    {
        public BiomeScatterParameter Scatter { get; set; } 
        public short Feature { get; set; } 
        public short Identifier { get; set; } 
        public short Pass { get; set; } 
        public bool CanUseInternal { get; set; } 
    }

    
    public struct BiomeClimate
    {
        public float Temperature { get; set; } 
        public float Downfall { get; set; } 
        public float RedSporeDensity { get; set; } 
        public float BlueSporeDensity { get; set; } 
        public float AshDensity { get; set; } 
        public float WhiteAshDensity { get; set; } 
        public float SnowAccumulationMin { get; set; } 
        public float SnowAccumulationMax { get; set; } 
    }

    
    public struct BiomeChunkGeneration
    {
        public Optional<BiomeClimate> Climate { get; set; } 
        public Optional<BiomeConsolidatedFeature[]> ConsolidatedFeatures { get; set; } 
        public Optional<BiomeMountainParameters> MountainParameters { get; set; } 
        public Optional<BiomeElementData[]> SurfaceMaterialAdjustments { get; set; } 
        public Optional<BiomeSurfaceMaterial> SurfaceMaterials { get; set; } 
        public bool HasSwampSurface { get; set; } 
        public bool HasFrozenOceanSurface { get; set; } 
        public bool HasEndSurface { get; set; } 
        public Optional<BiomeMesaSurface> MesaSurface { get; set; } 
        public Optional<BiomeCappedSurface> CappedSurface { get; set; } 
        public Optional<BiomeOverworldRules> OverworldRules { get; set; } 
        public Optional<BiomeMultiNoiseRules> MultiNoiseRules { get; set; } 
        public Optional<BiomeConditionalTransformation[]> LegacyRules { get; set; } 
        public Optional<BiomeReplacementData[]> ReplacementsData { get; set; }

	}

    
    public struct BiomeDefinition
    {
        public short NameIndex { get; set; } 
        public short BiomeID { get; set; } 
        public float Temperature { get; set; } 
        public float Downfall { get; set; } 
        public float RedSporeDensity { get; set; } 
        public float BlueSporeDensity { get; set; } 
        public float AshDensity { get; set; } 
        public float WhiteAshDensity { get; set; } 
        public float Depth { get; set; } 
        public float Scale { get; set; } 
        public int MapWaterColour { get; set; } 
        public bool Rain { get; set; } 
        public Optional<ushort[]> Tags { get; set; } 
        public Optional<BiomeChunkGeneration> ChunkGeneration { get; set; } 
    }
}