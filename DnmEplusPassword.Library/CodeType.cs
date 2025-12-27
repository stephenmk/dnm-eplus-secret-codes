namespace DnmEplusPassword.Library;

public enum CodeType
{
    Famicom = 0,     // NES
    NPC = 1,         // Original NPC Code
    Card_E = 2,      // NOTE: This can only be sent to villagers in a letter.
    Magazine = 3,    // Contest?
    User = 4,        // Player-to-Player
    Card_E_Mini = 5, // Only one data strip? Hit rate index must be set to 4.
    New_NPC = 6,     // Using the new password system?
    Monument = 7,    // Town Decorations (from Object Delivery Service, see: https://www.nintendo.co.jp/ngc/gaej/obje/)
}
