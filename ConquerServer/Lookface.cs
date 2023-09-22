using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConquerServer
{
    public class Lookface : JsonConverter<Lookface>
    {
        //genders
        public const int NoSex = 0;
        public const int MaleSex = 1;
        public const int FemaleSex = 2;
        //models
        public const int NoModel = 0;
        public const int SmallFemaleModel = 1;
        public const int LargeFemaleModel = 2;
        public const int SmallMaleModel= 3;
        public const int LargeMaleModel = 4;
        public const int MaleGhostModel = 98;
        public const int FemaleGhostModel = 0;

        //
        // Goals:
        // - Need properties OverlappingModel, Avatar, Sex, Model
        // - Should have both Get/Sets
        // - Should be able to convert LookfaceModel -> int
        // - Should be able to convert int -> LookfaceModel
        // - Should implement JsonConverter<LookfaceModel>

        public int MaskModel { get; private set; }
        public int Avatar { get; private set; }
        public int Sex { get; private set; }
        public int Model { get; private set; }

        public Lookface()
            : this(NoModel, NoSex, 0)
        {

        }

        public Lookface(int model, int sex, int avatar, int mask = NoModel)
        {
            MaskModel = mask;
            Avatar = avatar;
            Sex = sex;
            Model = model;
        }

        public Lookface(uint lookface)
        {
            MaskModel = (int)(lookface / 10000000);
            Avatar = (int)((lookface / 10000) % 1000); //--103
            Sex = (int)((lookface / 1000) % 10);// -- --- 1
            Model = (int)(lookface % 1000);
        }

        public uint ToUInt32()
        {
            return (uint)Model +
                 ((uint)Sex * 1000) +
                ((uint)Avatar * 10000) +
                ((uint)MaskModel * 10000000);
        }

        public Lookface ToGhost()
        {
            int mask = (Sex == MaleSex) ? MaleGhostModel : (Sex == FemaleGhostModel) ? FemaleGhostModel : NoModel;
            return new Lookface(Model, Sex,Avatar, mask);
        }

        public Lookface Normalize() //become alive
        {
            return new Lookface(Model, Sex, Avatar, NoModel);
        }

        public override Lookface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Lookface(reader.GetUInt32());
        }

        public override void Write(Utf8JsonWriter writer, Lookface value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Normalize().ToUInt32());
        }

        public static explicit operator uint(Lookface lookface)
        {
            return lookface.ToUInt32();
        }

        public static explicit operator Lookface(uint lookface)
        {
            return new Lookface(lookface);
        }
    }
}
