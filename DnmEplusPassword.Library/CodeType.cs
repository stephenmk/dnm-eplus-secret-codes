namespace DnmEplusPassword.Library;

public enum CodeType
{
    Famicom,     // NES
    NPC,         // Original NPC Code
    Card_E,      // NOTE: This can only be sent to villagers in a letter.
    Magazine,    // Contest?
    User,        // Player-to-Player
    Card_E_Mini, // Only one data strip? Hit rate index must be set to 4.
    New_NPC,     // Using the new password system?
    Monument,    // Town Decorations (from Object Delivery Service, see: https://www.nintendo.co.jp/ngc/gaej/obje/)
}
