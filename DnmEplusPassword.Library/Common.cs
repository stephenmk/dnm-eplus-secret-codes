using System.Collections.Immutable;
using System.Text;

namespace DnmEplusPassword.Library;

public static class Common
{
    public static readonly string AFe_Characters =
        """
        あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみ !"むめ%&'()~♥,-.♪0123456789:🌢<+>?@ABCDEFGHIJKLMNOPQRSTUVWXYZも💢やゆ_よabcdefghijklmnopqrstuvwxyzらりるれ�□。｢｣、･ヲァィゥェォャュョッーアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワンヴ☺ろわをんぁぃぅぇぉゃゅょっ⏎ガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽ
        """;

    public static readonly ImmutableArray<Rune> AFe_CharList = [.. AFe_Characters.EnumerateRunes()];

    public static readonly ImmutableArray<int> mMpswd_prime_number =
    [
        0x0011, 0x0013, 0x0017, 0x001D, 0x001F, 0x0025, 0x0029, 0x002B, 0x002F, 0x0035, 0x003B, 0x003D, 0x0043, 0x0047, 0x0049, 0x004F,
        0x0053, 0x0059, 0x0061, 0x0065, 0x0067, 0x006B, 0x006D, 0x0071, 0x007F, 0x0083, 0x0089, 0x008B, 0x0095, 0x0097, 0x009D, 0x00A3,
        0x00A7, 0x00AD, 0x00B3, 0x00B5, 0x00BF, 0x00C1, 0x00C5, 0x00C7, 0x00D3, 0x00DF, 0x00E3, 0x00E5, 0x00E9, 0x00EF, 0x00F1, 0x00FB,
        0x0101, 0x0107, 0x010D, 0x010F, 0x0115, 0x0119, 0x011B, 0x0125, 0x0133, 0x0137, 0x0139, 0x013D, 0x014B, 0x0151, 0x015B, 0x015D,
        0x0161, 0x0167, 0x016F, 0x0175, 0x017B, 0x017F, 0x0185, 0x018D, 0x0191, 0x0199, 0x01A3, 0x01A5, 0x01AF, 0x01B1, 0x01B7, 0x01BB,
        0x01C1, 0x01C9, 0x01CD, 0x01CF, 0x01D3, 0x01DF, 0x01E7, 0x01EB, 0x01F3, 0x01F7, 0x01FD, 0x0209, 0x020B, 0x021D, 0x0223, 0x022D,
        0x0233, 0x0239, 0x023B, 0x0241, 0x024B, 0x0251, 0x0257, 0x0259, 0x025F, 0x0265, 0x0269, 0x026B, 0x0277, 0x0281, 0x0283, 0x0287,
        0x028D, 0x0293, 0x0295, 0x02A1, 0x02A5, 0x02AB, 0x02B3, 0x02BD, 0x02C5, 0x02CF, 0x02D7, 0x02DD, 0x02E3, 0x02E7, 0x02EF, 0x02F5,
        0x02F9, 0x0301, 0x0305, 0x0313, 0x031D, 0x0329, 0x032B, 0x0335, 0x0337, 0x033B, 0x033D, 0x0347, 0x0355, 0x0359, 0x035B, 0x035F,
        0x036D, 0x0371, 0x0373, 0x0377, 0x038B, 0x038F, 0x0397, 0x03A1, 0x03A9, 0x03AD, 0x03B3, 0x03B9, 0x03C7, 0x03CB, 0x03D1, 0x03D7,
        0x03DF, 0x03E5, 0x03F1, 0x03F5, 0x03FB, 0x03FD, 0x0407, 0x0409, 0x040F, 0x0419, 0x041B, 0x0425, 0x0427, 0x042D, 0x043F, 0x0443,
        0x0445, 0x0449, 0x044F, 0x0455, 0x045D, 0x0463, 0x0469, 0x047F, 0x0481, 0x048B, 0x0493, 0x049D, 0x04A3, 0x04A9, 0x04B1, 0x04BD,
        0x04C1, 0x04C7, 0x04CD, 0x04CF, 0x04D5, 0x04E1, 0x04EB, 0x04FD, 0x04FF, 0x0503, 0x0509, 0x050B, 0x0511, 0x0515, 0x0517, 0x051B,
        0x0527, 0x0529, 0x052F, 0x0551, 0x0557, 0x055D, 0x0565, 0x0577, 0x0581, 0x058F, 0x0593, 0x0595, 0x0599, 0x059F, 0x05A7, 0x05AB,
        0x05AD, 0x05B3, 0x05BF, 0x05C9, 0x05CB, 0x05CF, 0x05D1, 0x05D5, 0x05DB, 0x05E7, 0x05F3, 0x05FB, 0x0607, 0x060D, 0x0611, 0x0617,
        0x061F, 0x0623, 0x062B, 0x062F, 0x063D, 0x0641, 0x0647, 0x0649, 0x064D, 0x0653, 0x0655, 0x065B, 0x0665, 0x0679, 0x067F, 0x0683,
    ];

    public static readonly ImmutableArray<byte> usable_to_fontnum_new_translation =
    [
        0x62, 0x4B, 0x7A, 0x35, 0x63, 0x71, 0x59, 0x5A, 0x4F, 0x64, 0x74, 0x36, 0x6E, 0x6C, 0x42, 0x79,
        0x6F, 0x38, 0x34, 0x4C, 0x6B, 0x25, 0x41, 0x51, 0x6D, 0x44, 0x50, 0x49, 0x37, 0x26, 0x52, 0x73,
        0x77, 0x55, 0x21, 0x72, 0x33, 0x45, 0x78, 0x4D, 0x43, 0x40, 0x65, 0x39, 0x67, 0x76, 0x56, 0x47,
        0x75, 0x4E, 0x69, 0x58, 0x57, 0x66, 0x54, 0x4A, 0x46, 0x53, 0x48, 0x70, 0x32, 0x61, 0x6A, 0x68,
    ];

    public static readonly ImmutableArray<byte> usable_to_fontnum_new =
    [
        0x0A, 0x1F, 0x1D, 0xF0, 0xF1, 0xF5, 0x0D, 0x05, 0xF2, 0x1E, 0xE7, 0x60, 0xEB, 0x11, 0x17, 0x04,
        0xED, 0x15, 0x23, 0xE9, 0xE8, 0xEF, 0x16, 0x10, 0x09, 0xF4, 0xC2, 0x12, 0xF8, 0xC0, 0x0F, 0xC3,
        0xF7, 0x5B, 0x7B, 0x5E, 0x08, 0x00, 0x19, 0x02, 0xF9, 0x24, 0x1A, 0x0C, 0xEC, 0x7C, 0x0E, 0xEA,
        0x01, 0x13, 0x07, 0x7E, 0x18, 0xF3, 0x14, 0x1C, 0x5D, 0x03, 0xEE, 0x1B, 0x0B, 0x7D, 0xC1, 0x06,
    ];

    public static readonly ImmutableArray<int> key_idx = [0x16, 0x6];

    public static readonly ImmutableArray<ImmutableArray<string>> mMpswd_transposition_cipher_char_table =
    [[
        "NiiMasaru",            // Animal Crossing programmer (worked on the original N64 title)
        "KomatsuKunihiro",      // Animal Crossing programmer (AF, AF+, AC, AFe+)
        "TakakiGentarou",       // Animal Crossing programmer
        "MiyakeHiromichi",      // Animal Crossing programmer
        "HayakawaKenzo",        // Animal Crossing programmer
        "KasamatsuShigehiro",   // Animal Crossing programmer
        "SumiyoshiNobuhiro",    // Animal Crossing programmer
        "NomaTakafumi",         // Animal Crossing programmer
        "EguchiKatsuya",        // Animal Crossing director
        "NogamiHisashi",        // Animal Crossing director
        "IidaToki",             // Animal Crossing screen designer
        "IkegawaNoriko",        // Animal Crossing character design
        "KawaseTomohiro",       // Animal Crossing NES/Famicom emulator programmer
        "BandoTaro",            // Animal Crossing Sound Effects programmer
        "TotakaKazuo",          // Animal Crossing Sound Director (Kazumi Totaka)
        "WatanabeKunio",        // Animal Crossing Script member (made text?)
    ], [
        "RichAmtower",          // Localization Manager @ Nintendo of America https://www.linkedin.com/in/rich-amtower-83222a1, https://nintendo.fandom.com/wiki/Rich_Amtower
        "KyleHudson",           // Former Product Testing Manager @ Nintendo of America https://metroid.fandom.com/wiki/Kyle_Hudson
        "MichaelKelbaugh",      // Debugger & Beta Tester @ Nintendo of America https://nintendo.fandom.com/wiki/Michael_Kelbaugh
        "RaycholeLAneff",       // Raychole L'Anett - Director of Engineering Services @ Nintendo of America https://metroid.fandom.com/wiki/Raychole_L%27Anett
        "LeslieSwan",           // Senior Editor @ Nintendo Power, VA, Nintendo of America localization manager @ Treehouse. https://www.mariowiki.com/Leslie_Swan
        "YoshinobuMantani",     // Nintendo of America employee (QA, Debugger) https://www.imdb.com/name/nm1412191/
        "KirkBuchanan",         // Senior Product Testing Manager @ Nintendo of America https://leadferret.com/directory/person/kirk-buchanan/16977208
        "TimOLeary",            // Localization Manager & Translator @ Nintendo of America https://nintendo.fandom.com/wiki/Tim_O%27Leary
        "BillTrinen",           // Senior Product Marketing Manager, Translator, & Interpreter @ Nintendo of America https://en.wikipedia.org/wiki/Bill_Trinen
        "nAkAyOsInoNyuuSankin", // Translates to "good bacteria" (善玉菌)
        "zendamaKINAKUDAMAkin", // Translates to "bad bacteria" (悪玉菌)
        "OishikutetUYOKUNARU",  // Translates to "It's becoming really delicious." "It's becoming strongly delicious."
        "AsetoAminofen",        // Translates to Acetaminophen. Like the drug.
        "fcSFCn64GCgbCGBagbVB", // fc = Famicom | SFC = Super Famicom | n64 = Nintendo 64 | GC = GameCube | gb = GameBoy | CGB = GameBoy Color | agb = GameBoy Advance | VB = Virtual Boy
        "YossyIsland",          // Yoshi's Island. The game.
        "KedamonoNoMori",       // Translates to "Animal Forest" or "Beast Forest"
    ]];

    public static readonly ImmutableArray<byte> mMpswd_chg_code_table =
    [
        0xF0, 0x83, 0xFD, 0x62, 0x93, 0x49, 0x0D, 0x3E, 0xE1, 0xA4, 0x2B, 0xAF, 0x3A, 0x25, 0xD0, 0x82,
        0x7F, 0x97, 0xD2, 0x03, 0xB2, 0x32, 0xB4, 0xE6, 0x09, 0x42, 0x57, 0x27, 0x60, 0xEA, 0x76, 0xAB,
        0x2D, 0x65, 0xA8, 0x4D, 0x8B, 0x95, 0x01, 0x37, 0x59, 0x79, 0x33, 0xAC, 0x2F, 0xAE, 0x9F, 0xFE,
        0x56, 0xD9, 0x04, 0xC6, 0xB9, 0x28, 0x06, 0x5C, 0x54, 0x8D, 0xE5, 0x00, 0xB3, 0x7B, 0x5E, 0xA7,
        0x3C, 0x78, 0xCB, 0x2E, 0x6D, 0xE4, 0xE8, 0xDC, 0x40, 0xA0, 0xDE, 0x2C, 0xF5, 0x1F, 0xCC, 0x85,
        0x71, 0x3D, 0x26, 0x74, 0x9C, 0x13, 0x7D, 0x7E, 0x66, 0xF2, 0x9E, 0x02, 0xA1, 0x53, 0x15, 0x4F,
        0x51, 0x20, 0xD5, 0x39, 0x1A, 0x67, 0x99, 0x41, 0xC7, 0xC3, 0xA6, 0xC4, 0xBC, 0x38, 0x8C, 0xAA,
        0x81, 0x12, 0xDD, 0x17, 0xB7, 0xEF, 0x2A, 0x80, 0x9D, 0x50, 0xDF, 0xCF, 0x89, 0xC8, 0x91, 0x1B,
        0xBB, 0x73, 0xF8, 0x14, 0x61, 0xC2, 0x45, 0xC5, 0x55, 0xFC, 0x8E, 0xE9, 0x8A, 0x46, 0xDB, 0x4E,
        0x05, 0xC1, 0x64, 0xD1, 0xE0, 0x70, 0x16, 0xF9, 0xB6, 0x36, 0x44, 0x8F, 0x0C, 0x29, 0xD3, 0x0E,
        0x6F, 0x7C, 0xD7, 0x4A, 0xFF, 0x75, 0x6C, 0x11, 0x10, 0x77, 0x3B, 0x98, 0xBA, 0x69, 0x5B, 0xA3,
        0x6A, 0x72, 0x94, 0xD6, 0xD4, 0x22, 0x08, 0x86, 0x31, 0x47, 0xBE, 0x87, 0x63, 0x34, 0x52, 0x3F,
        0x68, 0xF6, 0x0F, 0xBF, 0xEB, 0xC0, 0xCE, 0x24, 0xA5, 0x9A, 0x90, 0xED, 0x19, 0xB8, 0xB5, 0x96,
        0xFA, 0x88, 0x6E, 0xFB, 0x84, 0x23, 0x5D, 0xCD, 0xEE, 0x92, 0x58, 0x4C, 0x0B, 0xF7, 0x0A, 0xB1,
        0xDA, 0x35, 0x5F, 0x9B, 0xC9, 0xA9, 0xE7, 0x07, 0x1D, 0x18, 0xF3, 0xE3, 0xF1, 0xF4, 0xCA, 0xB0,
        0x6B, 0x30, 0xEC, 0x4B, 0x48, 0x1C, 0xAD, 0xE2, 0x21, 0x1E, 0xA2, 0xBD, 0x5A, 0xD8, 0x43, 0x7A,
    ];

    public static readonly ImmutableArray<ImmutableArray<int>> mMpswd_select_idx_table =
    [
        [0x11, 0x0B, 0x00, 0x14, 0x0E, 0x06, 0x08, 0x04],
        [0x05, 0x08, 0x0B, 0x10, 0x04, 0x06, 0x09, 0x13],
        [0x09, 0x0E, 0x11, 0x15, 0x0B, 0x0A, 0x13, 0x02],
        [0x00, 0x02, 0x01, 0x04, 0x12, 0x0A, 0x0B, 0x08],
        [0x11, 0x13, 0x10, 0x14, 0x0E, 0x08, 0x02, 0x09],
        [0x10, 0x02, 0x01, 0x08, 0x12, 0x04, 0x07, 0x06],
        [0x13, 0x06, 0x0A, 0x11, 0x01, 0x10, 0x08, 0x09],
        [0x11, 0x07, 0x12, 0x10, 0x0F, 0x02, 0x0B, 0x00],
        [0x06, 0x02, 0x0B, 0x01, 0x08, 0x0E, 0x00, 0x10],
        [0x13, 0x10, 0x0B, 0x08, 0x11, 0x02, 0x06, 0x0E],
        [0x12, 0x0F, 0x02, 0x07, 0x0A, 0x0B, 0x01, 0x0E],
        [0x08, 0x00, 0x0E, 0x02, 0x14, 0x0B, 0x0F, 0x11],
        [0x09, 0x01, 0x02, 0x00, 0x13, 0x08, 0x0E, 0x0A],
        [0x0A, 0x0B, 0x0E, 0x10, 0x13, 0x07, 0x11, 0x08],
        [0x13, 0x08, 0x06, 0x01, 0x11, 0x09, 0x0E, 0x0A],
        [0x09, 0x07, 0x11, 0x0E, 0x13, 0x0A, 0x01, 0x0B],
    ];

    // Methods
    public static byte mMpswd_chg_password_font_code_sub(byte character, in ImmutableArray<byte> fontnum_tbl)
    {
        for (byte i = 0; i < 0x40; i++)
        {
            if (fontnum_tbl[i] == character)
            {
                return i;
            }
        }
        return 0xFF;
    }

    public static void mMpswd_chg_password_font_code(ref byte[] password, in ImmutableArray<byte> fontnum_tbl)
    {
        for (int i = 0; i < 32; i++)
        {
            password[i] = mMpswd_chg_password_font_code_sub(password[i], fontnum_tbl);
        }
    }

    public static void mMpswd_transposition_cipher(ref byte[] data, bool negate, int keyIndex)
    {
        var multiplier = negate ? -1 : 1;
        var key = data[key_idx[keyIndex]];
        var transpositionTable = mMpswd_transposition_cipher_char_table[keyIndex];
        var transpositionCipher = transpositionTable[key & 0x0F];

        int cipherIndex = 0;

        for (int i = 0; i < 24; i++)
        {
            if (i != key_idx[keyIndex])
            {
                int valueModifier = (transpositionCipher[cipherIndex++] * multiplier) & 0xFF;
                data[i] = (byte)(data[i] + valueModifier);
                if (cipherIndex >= transpositionCipher.Length)
                {
                    cipherIndex = 0;
                }
            }
        }
    }

    public static void mMpswd_bit_reverse(ref byte[] data)
    {
        for (int i = 0; i < 24; i++)
        {
            if (i != 1)
            {
                data[i] ^= 0xFF;
            }
        }
    }

    public static void mMpswd_bit_arrange_reverse(ref byte[] data)
    {
        byte[] buffer = new byte[23];
        byte[] outputBuffer = new byte[23];
        for (int i = 0, idx = 0; i < 24; i++)
        {
            if (i != 1)
            {
                buffer[idx++] = data[i];
            }
        }

        for (int i = 0; i < 23; i++) // pretty sure this should be < 23
        {
            byte value = buffer[22 - i]; // this should be 22
            byte changedValue = 0;
            for (var x = 0; x < 8; x++)
            {
                changedValue |= (byte)(((value >> x) & 1) << (7 - x));
            }

            outputBuffer[i] = changedValue;
        }

        for (int i = 0, idx = 0; i < 23; i++)
        {
            if (i == 1)
            {
                idx++;
            }
            data[idx++] = outputBuffer[i];
        }
    }

    public static void mMpswd_bit_shift(ref byte[] data, int shift)
    {
        byte[] buffer = new byte[23];
        for (int i = 0, idx = 0; i < 24; i++)
        {
            if (i != 1)
            {
                buffer[idx++] = data[i];
            }
        }

        byte[] outputBuffer = new byte[23];

        if (shift > 0)
        {
            int destinationPosition = shift / 8;
            int destinationOffset = shift % 8;

            for (int i = 0; i < 23; i++)
            {
                outputBuffer[(i + destinationPosition) % 23] = (byte)((buffer[i] << destinationOffset)
                    | (buffer[(i + 22) % 23] >> (8 - destinationOffset)));
            }

            // Copy to original buffer
            for (int i = 0, idx = 0; i < 23; i++)
            {
                if (i == 1) // Skip copying the second byte
                {
                    idx++;
                }
                data[idx++] = outputBuffer[i];
            }
        }
        else if (shift < 0)
        {
            for (int i = 0; i < 23; i++)
            {
                outputBuffer[i] = buffer[22 - i];
            }
            shift = -shift;

            int destinationPosition = shift / 8;
            int destinationOffset = shift % 8;

            for (int i = 0; i < 23; i++)
            {
                buffer[(i + destinationPosition) % 23] = outputBuffer[i];
            }

            for (int i = 0; i < 23; i++)
            {
                outputBuffer[i] = (byte)((buffer[i] >> destinationOffset) | ((buffer[(i + 22) % 23]) << (8 - destinationOffset)));
            }

            for (int i = 0, idx = 0; i < 23; i++)
            {
                if (i == 1)
                {
                    idx++;
                }
                data[idx++] = outputBuffer[22 - i];
            }
        }
    }

    public static bool mMpswd_new_password_zuru_check(
        int checksum,
        int CodeType,
        string recipient,
        string townName,
        string sender,
        ushort itemId,
        int npcCode,
        int unknown)
    {
        if (CodeType == 2 || CodeType >= 8)
        {
            return true;
        }

        var invalid = true;

        var calculatedChecksum = 0;
        calculatedChecksum += GetStringByteValue(recipient);
        calculatedChecksum += GetStringByteValue(townName);
        calculatedChecksum += GetStringByteValue(sender);
        calculatedChecksum += itemId;

        if ((calculatedChecksum & 0xF) == checksum &&
            mMpswd_check_default_hit_rate(CodeType, npcCode) &&
            mMpswd_check_default_npc_code(CodeType, npcCode, unknown))
        {
            invalid = false;
        }

        return invalid;
    }

    private static bool mMpswd_check_default_hit_rate(int codeType, int codeCheck)
    {
        bool hitRate = false;
        if (codeType == 3 && codeCheck < 5)
        {
            hitRate = true;
        }
        else if (codeCheck == 4)
        {
            hitRate = true;
        }
        return hitRate;
    }

    private static bool mMpswd_check_default_npc_code(int codeType, int npcCode, int r3_9)
    {
        bool valid = false;
        if (codeType >= 5)
        {
            if (codeType == 7)
            {
                if (npcCode == 0xFF)
                {
                    valid = true;
                }
            }
            else
            {
                valid = true;
            }
        }
        else
        {
            if (codeType == 1)
            {
                valid = true;
            }
            else if (codeType >= 0)
            {
                if (r3_9 == 0 && npcCode == 0xFF)
                {
                    valid = true;
                }
            }
        }

        return valid;
    }

    // Custom Functions \\
    public static int GetPasswordChecksum(byte[] passwordData)
    {
        int checksum = 0;

        for (int i = 0x03; i < 0x15; i++)
        {
            checksum += passwordData[i];
        }

        checksum += (passwordData[0x15] << 8) | passwordData[16];
        checksum += passwordData[2];

        return (((checksum >> 2) & 3) << 2) | (((checksum << 6) & 0xC0) >> 6);
    }

    public static bool VerifyChecksum(byte[] passwordData)
    {
        int calculatedChecksum = GetPasswordChecksum(passwordData);
        int storedChecksum = ((passwordData[0] & 3) << 2) | ((passwordData[1] & 0xC0) >> 6);

        Console.WriteLine($"Calculated Checksum: 0x{calculatedChecksum:X}\nStored Checksum: 0x{storedChecksum:X}");

        return calculatedChecksum == storedChecksum;
    }

    public static byte[] StringToAFByteArray(string input)
    {
        var inputRunes = input.EnumerateRunes().ToArray();
        var output = new byte[inputRunes.Length];

        for (int i = 0; i < input.Length; i++)
        {
            var rune = inputRunes[i];
            var idx = AFe_CharList.IndexOf(rune);
            if (idx < 0)
            {
                throw new ArgumentException($"Invalid character: {rune}", nameof(input));
            }
            output[i] = (byte)idx;
        }

        return output;
    }

    private static int GetStringByteValue(string input)
        => StringToAFByteArray(input)
            .Aggregate
            (
                seed: 0,
                func: static (current, t) => current + t
            );
}
