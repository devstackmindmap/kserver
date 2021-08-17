using AkaDB.MySql;
using System;
using AkaEnum;

namespace Common.Entities.Item
{
    public static class PieceFactory
    {
        public static IPiece CreatePiece(PieceType pieceType, uint userId, uint pieceId, DBContext db, int count = 0)
        {
            switch (pieceType)
            {
                case PieceType.Unit:
                    return new UnitPiece(userId, pieceId, db, count);
                case PieceType.Card:
                    return new CardPiece(userId, pieceId, db, count);
                case PieceType.Weapon:
                    return new WeaponPiece(userId, pieceId, db, count);
                case PieceType.SquareObject:
                    return new SquareObjectPiece(userId,  db, count);
                case PieceType.SquareObjectCore:
                    return new SquareObjectCorePiece(userId,  db, count);
                case PieceType.SquareObjectAgency:
                    return new SquareObjectAgencyPiece(userId,  db, count);
                default:
                    throw new Exception("Non existent type");
            }
        }

        public static Equipment CreateEquipment(PieceType pieceType, uint userId, uint pieceId, DBContext db, int count = 0)
        {
            switch (pieceType)
            {
                case PieceType.Weapon:
                    return new WeaponPiece(userId, pieceId, db, count);
                default:
                    throw new Exception("Non existent type");
            }
        }
    }
}
