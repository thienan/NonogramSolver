﻿using NonogramSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static NonogramSolver.Solver.Cell;

namespace NonogramSolver.Tests
{
    public class RowSolverTests
    {
        private IList<Cell> MakeEmptyRow(int length)
        {
            var result = new List<Cell>(length);
            for (int i = 0; i < length; i++)
            {
                result.Add(new Cell() { State = CellState.Undefined });
            }
            return result;
        }

        private IList<Cell> MakeList(params CellState[] cellStates)
        {
            var result = new List<Cell>(cellStates.Length);
            for (int i = 0; i < cellStates.Length; i++)
            {
                result.Add(new Cell() { State = cellStates[i] });
            }
            return result;
        }

        private RowDescriptor MakeDescriptor(params int[] ranges)
        {

            return new RowDescriptor
            {
                BlockSizes = ranges.ToList()
            };
        }

        [Theory]
        [InlineData(3, new int[] { 3 }, new CellState[] { CellState.Filled, CellState.Filled, CellState.Filled })]
        [InlineData(3, new int[] { 1, 1 }, new CellState[] { CellState.Filled, CellState.Empty, CellState.Filled })]
        [InlineData(6, new int[] { 2, 1, 1 }, new CellState[] { CellState.Filled, CellState.Filled, CellState.Empty,
                                        CellState.Filled, CellState.Empty,
                                        CellState.Filled })]
        [InlineData(6, new int[] { 5 }, new CellState[] { CellState.Undefined, CellState.Filled, CellState.Filled,
                                        CellState.Filled, CellState.Filled, CellState.Undefined })]

        [InlineData(6, new int[] { 4 }, new CellState[] { CellState.Undefined, CellState.Undefined, CellState.Filled,
                                        CellState.Filled,CellState.Undefined, CellState.Undefined })]
        [InlineData(6, new int[] { 3, 1 }, new CellState[] { CellState.Undefined, CellState.Filled, CellState.Filled,
                                        CellState.Undefined,CellState.Undefined, CellState.Undefined })]
        [InlineData(6, new int[] { 2, 2 }, new CellState[] { CellState.Undefined, CellState.Filled, CellState.Undefined,
                                        CellState.Undefined,CellState.Filled, CellState.Undefined })]
        [InlineData(6, new int[] { 1 }, new CellState[] { CellState.Undefined, CellState.Undefined, CellState.Undefined,
                                        CellState.Undefined, CellState.Undefined, CellState.Undefined })]
        public void fill_invariant_cells(int rowSize, int[] ranges, CellState[] resultStates)
        {
            var initState = MakeEmptyRow(rowSize);
            var solver = new RowSolver();

            solver.FillInvariantCells(initState, MakeDescriptor(ranges));
            Assert.Equal(MakeList(resultStates), initState);
        }


        [Theory]
        [InlineData(new int[] { 3 }, new CellState[] { CellState.Filled, CellState.Filled , CellState.Filled }, RowStatus.FilledCorrectly)]
        [InlineData(new int[] { 3 }, new CellState[] { CellState.Empty, CellState.Filled, CellState.Filled }, RowStatus.ContainsErrors)]
        [InlineData(new int[] { 3 }, new CellState[] { CellState.Empty, CellState.Filled, CellState.Empty }, RowStatus.ContainsErrors)]
        [InlineData(new int[] { 1,1 }, new CellState[] { CellState.Filled, CellState.Empty, CellState.Filled }, RowStatus.FilledCorrectly)]
        [InlineData(new int[] { 1, 1 }, new CellState[] { CellState.Filled, CellState.Filled, CellState.Filled }, RowStatus.ContainsErrors)]
        public void calculate_row_status(int[] ranges, CellState[] states, RowStatus resultStatus)
        {
            var row = MakeList(states);
            var solver = new RowSolver();
            var descriptor = MakeDescriptor(ranges);

            Assert.Equal(resultStatus, solver.GetRowStatus(row, descriptor));
        }
    }
}