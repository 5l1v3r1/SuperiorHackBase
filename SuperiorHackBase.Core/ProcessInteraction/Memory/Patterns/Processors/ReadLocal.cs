﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperiorHackBase.Core.ProcessInteraction.Memory.Patterns.Processors
{
    [Processor(Pushes = 1)]
    public class ReadLocal : IPatternProcessor
    {
        public int Offset { get; private set; }
        public OperandType Type { get; private set; }

        public ReadLocal(int offset, OperandType type)
        {
            Offset = offset;
            Type = type;
        }

        public void Process(IHackContext context, PatternFinding finding, Stack<Pointer> operands, ScanResult result)
        {
            Pointer operand = Pointer.Zero;
            switch(Type)
            {
                case OperandType.i8:
                    operand = (int)finding.Data[Offset];
                    break;
                case OperandType.i16:
                    operand = new Pointer(BitConverter.ToInt16(finding.Data, Offset));
                    break;
                case OperandType.i32:
                    operand = new Pointer(BitConverter.ToInt32(finding.Data, Offset));
                    break;
                case OperandType.i64:
                    operand = new Pointer(BitConverter.ToInt64(finding.Data, Offset));
                    break;
                case OperandType.u16:
                    operand = new Pointer(BitConverter.ToUInt16(finding.Data, Offset));
                    break;
                case OperandType.u32:
                    operand = new Pointer(BitConverter.ToUInt32(finding.Data, Offset));
                    break;
                case OperandType.u64:
                    operand = new Pointer(BitConverter.ToUInt64(finding.Data, Offset));
                    break;
            }
            operands.Push(operand);
        }
    }
}
