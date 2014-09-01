using MinecraftModelExporter.GeometryProcessor.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor
{
    class Block
    {
        public ushort ID;
        public byte Metadata;
        public bool UseMetadata;
        public string Name;
        public bool UsesOneTexture = false;
        public string[] texture;

        public virtual string GetTextureForSide(BlockSide side, byte metadata)
        {
            return texture[GetSideInt(side)];
        }

        public virtual string[] GetTextures(byte metadata)
        {
            return texture;
        }

        public virtual BlockSide GetUsedTextureSides()
        {
            return BlockSide.AllSame;
        }

        public virtual bool IsTransparent()
        {
            return IsFullyCustomModel();
        }

        private static Block[] _blocks = new Block[ushort.MaxValue];
        public static Block[] Blocks
        {
            get { return _blocks; }
        }

        public static bool IsTransparent(BlockData data)
        {
            if (_blocks[data.GetGlobalID()] == null)
                return true;

            return _blocks[data.GetGlobalID()].IsTransparent();
        }

        public static bool CanBuild(BlockData data, BlockData me)
        {
            if (_blocks[data.GetGlobalID()] == null)
                return true;

            if (_blocks[data.GetGlobalID()].IsTransparent())
            { 
                return (data.ID != me.ID);
            }

            return true;
        }

        public static void Init()
        {
            //Tex: X+, X-, Y+, Y-, Z+, Z-
            AddBlock(new SolidBlock("Stone", 1, "stone"));
            AddBlock(new SolidBlock("Grass", 2, new string[] { "grass_side", "grass_side", "grass_top", "dirt", "grass_side", "grass_side" }));
            AddBlock(new SolidBlock("Dirt", 3, "dirt"));
            AddBlock(new SolidBlock("Cobblestone", 4, "cobblestone"));
            AddBlock(new WoodenPlanks(5));
            AddBlock(new Saplings(6, "Saplings", new TextureBuilder().AddForMetadata(0, "sapling_oak").AddForMetadata(1, "sapling_spruce").AddForMetadata(2, "sapling_birch").AddForMetadata(3, "sapling_jungle")));
            AddBlock(new SolidBlock("Bedrock", 7, "bedrock"));
            AddBlock(new Liquid("Water", 8, "water", true, 7));
            AddBlock(new Liquid("Water", 9, "water", false, 7));
            AddBlock(new Liquid("Lava", 10, "lava", true, 7));
            AddBlock(new Liquid("Lava", 11, "lava", false, 7));
            AddBlock(new SolidBlock("Sand", 12, "sand"));
            AddBlock(new SolidBlock("Gravel", 13, "gravel"));
            AddBlock(new SolidBlock("Gold ore", 14, "gold_ore"));
            AddBlock(new SolidBlock("Iron ore", 15, "iron_ore"));
            AddBlock(new SolidBlock("Coal ore", 16, "coal_ore"));
            AddBlock(new WoodTrunk(17));
            AddBlock(new Leaves(18));
            AddBlock(new SolidBlock("Sponge", 19, "sponge"));
            AddBlock(new Glass("Glass", 20, "glass"));
            AddBlock(new SolidBlock("Lapis ore", 21, "lapis_ore"));
            AddBlock(new SolidBlock("Lapis block", 22, "lapis_block"));
            AddBlock(new Dispenser(23));
            AddBlock(new Sandstone(24));
            AddBlock(new SolidBlock("Noteblock", 25, "noteblock"));
            AddBlock(new Bed(26)); 
            AddBlock(new Rails(27));
            AddBlock(new Rails(28));
            AddBlock(new StickyPiston(29, "Sticky piston"));
            AddBlock(new SimpleFlower(30, "Cobweb", "web"));
            AddBlock(new TallGrass(31, "Grass"));
            AddBlock(new SimpleFlower(32, "Dead bush", "deadbush"));
            AddBlock(new Piston(33, "Piston"));
            //Piston extension
            AddBlock(new Wool(35));
            AddBlock(new SolidBlock("Block moved by piston", 36, "BlockMovedByPiston_HasNoTexture"));
            AddBlock(new SimpleFlower(37, "Dandelion", "flower_dandelion"));
            AddBlock(new SimpleFlower(38, "Poppy", "flower_rose"));
            AddBlock(new SimpleFlower(39, "Brown mushroom", "mushroom_brown"));
            AddBlock(new SimpleFlower(40, "Red mushroom", "mushroom_red"));
            AddBlock(new SolidBlock("Block of gold", 41, "gold_block"));
            AddBlock(new SolidBlock("Block of iron", 42, "iron_block"));
            AddBlock(new MetadataSolidTextureBlock("Double stone slab", 43, new TextureBuilder().AddForMetadata(0, new TextureMask().AddData("stone_slab_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("stone_slab_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures).AddForMetadata(1, new TextureMask().AddData("sandstone_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("sandstone_normal", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures).AddForMetadata(2, "planks_oak").AddForMetadata(3, "cobblestone").AddForMetadata(4, "brick").AddForMetadata(5, "stonebrick").AddForMetadata(6, "nether_brick").AddForMetadata(7, "quartz_block_side")));
            AddBlock(new StoneSlab(44));
            AddBlock(new SolidBlock("Bricks", 45, "brick"));
            AddBlock(new SolidBlock("TNT", 46, new TextureMask().AddData("tnt_bottom", BlockTexture.Yneg).AddData("tnt_top", BlockTexture.Ypos).AddData("tnt_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures));
            AddBlock(new SolidBlock("Bookshelf", 47, new TextureMask().AddData("planks_oak", BlockTexture.Yneg | BlockTexture.Ypos).AddData("bookshelf", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures));
            AddBlock(new SolidBlock("Mossy stone", 48, "cobblestone_mossy"));
            AddBlock(new SolidBlock("Obsidian", 49, "obsidian"));
            AddBlock(new Torch(50));
            AddBlock(new Fire(51));
            AddBlock(new SolidBlock("Monster spawner", 52, "mob_spawner"));
            AddBlock(new Stairs(53, "planks_oak"));
            //54: chest
            //55: redstone wire
            AddBlock(new SolidBlock("Diamond ore", 56, "diamond_ore"));
            AddBlock(new SolidBlock("Block of diamond", 57, "diamond_block"));
            AddBlock(new SolidBlock("Crafting table", 58, new TextureMask().AddData("crafting_table_top", BlockTexture.Ypos).AddData("crafting_table_side", BlockTexture.Xneg | BlockTexture.Zpos | BlockTexture.Xpos).AddData("crafting_table_front", BlockTexture.Zneg).AddData("planks_oak", BlockTexture.Yneg).Textures));
            //59: wheat
            //60: farmland
            AddBlock(new Furnace(61));
            AddBlock(new FurnaceBurning(62));
            //63: standing sign
            //64: oak door
            AddBlock(new Ladder(65));
            AddBlock(new Rails(66));
            AddBlock(new Stairs(67, "cobblestone"));
            //68: wall sign
            //69: lever
            //70: stone pressure plate
            //71: iron door
            //72: wooden pressure plate
            AddBlock(new SolidBlock("Redstone ore", 73, "redstone_ore"));
            AddBlock(new SolidBlock("Glowing redstone ore", 74, "redstone_ore"));
            //75: redstone torch off
            //76: redstone torch on
            //77: stone button
            //78: snow
            AddBlock(new SolidBlock("Ice", 79, "ice"));
            AddBlock(new SolidBlock("Snow block", 80, "snow"));
            //81: cactus
            AddBlock(new SolidBlock("Clay", 82, "clay"));
            //83: sugar cannes
            AddBlock(new SolidBlock("Jukebox", 84, new TextureMask().AddData("jukebox_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Yneg | BlockTexture.Zneg | BlockTexture.Zpos).AddData("jukebox_top", BlockTexture.Ypos).Textures));
            AddBlock(new Fence(85, "planks_oak", "planks_oak"));
            AddBlock(new Pumpkin(86));
            AddBlock(new SolidBlock("Netherrack", 87, "netherrack"));
            AddBlock(new SolidBlock("Soul sand", 88, "soul_sand"));
            AddBlock(new SolidBlock("Glowstone", 89, "glowstone"));
            //90: nether portal
            AddBlock(new PumpkinLanterns(91));
            //92: cake
            //93: repeater off
            //94: repeater on
            //95: stained glasses
            //96: wooden trap door
            AddBlock(new SolidBlock("Stone monster egg", 97, "stone"));
            //98: stone bricks
            //99: red mushroom cap
            //100: brown mushroom cap
            AddBlock(new PlusShapedBlock(101, "Iron bars", "iron_bars"));
            AddBlock(new PlusShapedBlock(102, "Glass pan", "glass", "glass_pane_top"));
            AddBlock(new SolidBlock("Melon", 103, new TextureMask().AddData("melon_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("melon_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures));
            //104: pumpkin stem
            //105: melon stem
            //106: vines
            //107: oak fence gate
            AddBlock(new Stairs(108, "brick"));
            AddBlock(new Stairs(109, "stonebrick"));
            //110: mycelium
            AddBlock(new SolidBlock("Mycelium", 110, new TextureMask().AddData("micelyum_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("mycelium_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures));
            //111: lily pad
            AddBlock(new SolidBlock("Netherbrick", 112, "nether_brick"));
            //113: nether brick fence
            AddBlock(new Stairs(114, "nether_brick"));
            //115: nether wart
            //117: enchanment table
            //118: cauldron
            //119: end portal
            //120: end portal frame
            AddBlock(new SolidBlock("End stone", 121, "end_stone"));
            //122: dragon egg
            AddBlock(new SolidBlock("Redstone lamp", 123, "redstone_lamp_off"));
            AddBlock(new SolidBlock("Redstone lamp", 124, "redstone_lamp_on"));
            AddBlock(new MetadataSolidTextureBlock("Double wooden slab", 125, new TextureBuilder().AddForMetadata(0, "planks_oak").AddForMetadata(1, "planks_spruce").AddForMetadata(2, "planks_birch").AddForMetadata(3, "planks_jungle")));
            AddBlock(new WoodenSlab(126));
            //127: cocoa
            AddBlock(new Stairs(128, "sandstone_normal", "sandstone_top"));
            AddBlock(new SolidBlock("Emerald ore", 129, "emerald_ore"));
            //130: ender chest
            //131: tripwire hook
            //132: tripwire
            AddBlock(new SolidBlock("Emerald block", 133, "emerald_block"));
            AddBlock(new Stairs(134, "planks_spruce"));
            AddBlock(new Stairs(135, "planks_birch"));
            AddBlock(new Stairs(136, "planks_jungle"));
            AddBlock(new SolidBlock("Command block", 137, "command_block"));
            //138: beacon
            //139: cobblestone wall
            //140: flower pot
            //141: carrots
            //142: potatoes
            //143: wooden button
            //144: mob head
            //145: anvil
            //146: trapped chest
            //147: Weighted Pressure Plate (light)
            //148: Weighted Pressure Plate (heavy)
            //149: Redstone Comparator (inactive)
            //150: Redstone Comparator (active)
            //151: Daylight Sensor
            AddBlock(new SolidBlock("Block of redstone", 152, "redstone_block"));
            AddBlock(new SolidBlock("Nether quartz ore", 153, "quartz_ore"));
            //154: Hopper
            AddBlock(new QuartzBlock(155)); //Missing metadatas 2,3,4
            //156: Quartz stairs
            AddBlock(new Rails(157));
            AddBlock(new Dropper(158));
            //159: Stained Clay
            //160: Stained Glass Pane
            //161: Acacia Leaves
            //162: Acacia Wood
            //163: Acacia Wood Stairs
            //164: Dark Oak Wood Stairs
            //165: Slime Block
            //166: Barrier
            //167: Iron Trapdoor
            //168: Prismarine
            //169: Sea Lantern
            AddBlock(new SolidBlock("Hay Bale", 170, new TextureMask().AddData("hay_block_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("hay_block_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures));
            //171: Carpet
            AddBlock(new HardenedClay(172));
            AddBlock(new SolidBlock("Block of coal", 173, "coal_block"));
            AddBlock(new SolidBlock("Packed ice", 174, "ice"));
            //175: Sunflower
            //176: Free-standing Banner
            //177: Wall-mounted Banner
            //178: Inverted Daylight Sensor
            //179: Red Sandstone
            //180: Red Sandstone Stairs
            //181: Double Red Sandstone Slab
            //182: Red Sandstone Slab
            //183: Spruce Fence Gate
            //184: Birch Fence Gate
            //185: Jungle Fence Gate
            //186: Dark Oak Fence Gate
            //187: Acacia Fence Gate
            //188: Spruce Fence
            //189: Birch Fence
            //190: Jungle Fence
            //191: Dark Oak Fence
            //192: Acacia Fence
            //193: Spruce Door Block
            //194: Birch Door Block
            //195: Jungle Door Block
            //196: Acacia Door Block
            //197: Dark Oak Door Block
        }

        private static void AddBlock(Block b)
        {
            b.Name = b.Name.Replace(' ', '_');
            if (b.UseMetadata)
            {
                for (byte m = 0; m < byte.MaxValue; m++)
                {
                    _blocks[BlockData.GetGlobalID(b.ID, m)] = b;
                }
            }
            else
            {
                _blocks[BlockData.GetGlobalID(b.ID, b.Metadata)] = b;
            }
        }

        public static int GetSideInt(BlockSide side)
        {
            string s = side.ToString().ToLower();

            int i = 0;

            if (s.StartsWith("x"))
            {
                if (s.EndsWith("pos"))
                    return 0;
                if (s.EndsWith("neg"))
                    return 1;
            }
            if (s.StartsWith("y"))
            {
                if (s.EndsWith("pos"))
                    return 2;
                if (s.EndsWith("neg"))
                    return 3;
            }
            if (s.StartsWith("z"))
            {
                if (s.EndsWith("pos"))
                    return 4;
                if (s.EndsWith("neg"))
                    return 5;
            }

            return i;
        }

        public virtual bool IsFullSide(BlockSide side)
        {
            return true;
        }

        public virtual bool IsFullyCustomModel()
        {
            return false;
        }

        public virtual List<CustomBlockData> GenerateSide(BlockSide side, byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg)
        {
            return new List<CustomBlockData>();
        }

        public virtual List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            return new List<CustomBlockData>();
        }

        public virtual bool CanGenerateSide(BlockSide side, byte metadata)
        {
            return true;
        }

        public static BlockSide GetSideFromNormal(Vector3 norm)
        {
            if (norm.X > 0 && norm.Y == 0 && norm.Z == 0)
                return BlockSide.Xpos;
            if (norm.X < 0 && norm.Y == 0 && norm.Z == 0)
                return BlockSide.Xneg;

            if (norm.X == 0 && norm.Y > 0 && norm.Z == 0)
                return BlockSide.Ypos;
            if (norm.X == 0 && norm.Y < 0 && norm.Z == 0)
                return BlockSide.Yneg;

            if (norm.X == 0 && norm.Y == 0 && norm.Z > 0)
                return BlockSide.Zpos;
            if (norm.X == 0 && norm.Y == 0 && norm.Z < 0)
                return BlockSide.Zneg;

            return BlockSide.AllSame;
        }
    }

    [Flags]
    enum BlockSide : int
    {
        Xpos = 0,
        Xneg = 1,
        Ypos = 2,
        Yneg = 4,
        Zpos = 8,
        Zneg = 16,

        AllSame = 32,
    }

    [Flags]
    enum BlockTexture : int
    {
        Xpos = 1,
        Xneg = 2,
        Ypos = 4,
        Yneg = 8,
        Zpos = 16,
        Zneg = 32,

        AllSame = 64,
    }
}
