﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    // DynamicCache - класс для кэша (сказали надо) (не хочу разбираться зачем и что он делает)
    class DynamicCache
    {
        private Triangle[] Cache = new Triangle[4];

        //Текущий размер кэша
        private UInt32 Size = 2;

        //Треугольников в кэше
        private UInt32 InCache = 0;

        //Реальные размеры кэшируемого пространства
        private readonly ToolPoint SizeOfSpace;

        //Размеры одной ячейки кэша в пересчете на реальное пространство
        private double xSize;
        private double ySize;

        public DynamicCache(ToolPoint _sizeOfSpace)
        {
            SizeOfSpace = _sizeOfSpace;
            xSize = SizeOfSpace.x / (double)Size;
            ySize = SizeOfSpace.y / (double)Size;
        }

        public void Add(Triangle _T)
        {
            InCache++;

            if (InCache >= Cache.Length * 3)
                Increase();

            Cache[GetKey(_T.Centroid)] = _T;
        }
        public Triangle FindTriangle(ToolPoint _Point)
        {
            UInt32 key = GetKey(_Point);
            if (Cache[key] != null)
                return Cache[key];

            // Дополнительный поиск не null ячейки, ускоряет алгоритм 
            for (uint i = key - 25; i < key && i >= 0 && i < Cache.Length; i++)
                if (Cache[i] != null)
                    return Cache[i];

            for (uint i = key + 25; i > key && i >= 0 && i < Cache.Length; i--)
                if (Cache[i] != null)
                    return Cache[i];

            return null;
        }

        //Увеличивает размер кэша в 4 раза
        private void Increase()
        {
            Triangle[] NewCache = new Triangle[(Size * 2) * (Size * 2)];
            UInt32 newIndex;

            //Передача ссылок из старого кэша в новый
            for (UInt32 i = 0; i < Cache.Length; i++)
            {
                newIndex = GetNewIndex(i);
                NewCache[newIndex] = Cache[i];
                NewCache[newIndex + 1] = Cache[i];
                NewCache[newIndex + Size * 2] = Cache[i];
                NewCache[newIndex + Size * 2 + 1] = Cache[i];
            }

            Size *= 2;
            xSize = SizeOfSpace.x / (double)Size;
            ySize = SizeOfSpace.y / (double)Size;

            Cache = NewCache;
        }
        private UInt32 GetKey(ToolPoint _point)
        {
            UInt32 i = (UInt32)(_point.y / ySize);
            UInt32 j = (UInt32)(_point.x / xSize);

            if (i == Size)
                i--;
            if (j == Size)
                j--;

            return i * Size + j;
        }
        private UInt32 GetNewIndex(UInt32 _OldIndex)
        {
            UInt32 i = (_OldIndex / Size) * 2;
            UInt32 j = (_OldIndex % Size) * 2;

            return i * (Size * 2) + j;
        }
    }
}
