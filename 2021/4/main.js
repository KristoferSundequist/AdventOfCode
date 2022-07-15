const fs = require('fs');
const lines = fs.readFileSync('data.txt').toString().split("\n");
const verbose = false
const randomNumbers = lines[0].split(",").map(str => parseInt(str, 10))

function getBoards(lines) {
    let boards = []
    const boardPart = lines.slice(1)

    for (let i = 0; i < (boardPart.length / 6); i++) {
        boards.push(boardPart.slice(i * 6 + 1, (i + 1) * 6))
    }
    return boards.map(board => board.map(row => row.trim().split(/\s+/).map(str => parseInt(str, 10))))
}

const boards = getBoards(lines)
let bingoBoards = boards.map(_ => [...Array(5).keys()].map(_ => [...Array(5).keys()].map(_ => false)))
const numberIndexes = boards.map(board => {
    let index = {}
    for (let i = 0; i < 5; i++) {
        for (let j = 0; j < 5; j++) {
            index[board[i][j]] = [i, j]
        }
    }
    return index
})

function isBingo(board, row, col) {
    let isRowBingo = true
    let isColBingo = true
    for (let i = 0; i < 5; i++) {
        if (board[row][i] === false) {
            isRowBingo = false
        }
        if (board[i][col] === false) {
            isColBingo = false
        }
    }
    return isRowBingo || isColBingo
}

function getUnmarkedSum(boardIndex) {
    let sum = 0
    for (let row = 0; row < 5; row++) {
        for (let col = 0; col < 5; col++) {
            if (bingoBoards[boardIndex][row][col] === false) {
                sum += boards[boardIndex][row][col]
            }
        }
    }
    return sum
}

let haveBingoed = boards.map( _ => false)

for (let bingoNumber of randomNumbers) {
    if (verbose) {
        console.log("------------------------")
        console.log(bingoNumber)
    }
    for (let boardIndex = 0; boardIndex < boards.length; boardIndex++) {
        const coord = numberIndexes[boardIndex][bingoNumber]
        if (coord === undefined) {
            continue
        }
        const [row, col] = coord
        bingoBoards[boardIndex][row][col] = true
        if (verbose) {
            console.log("BOARD " + boardIndex)
            console.log("coord: " + JSON.stringify(coord) + "  destructed: " + row + ", " + col)
            console.log(JSON.stringify(bingoBoards[boardIndex]))
            console.log(JSON.stringify(boards[boardIndex]))
        }
        if (isBingo(bingoBoards[boardIndex], row, col) && haveBingoed[boardIndex] === false) {
            haveBingoed[boardIndex] = true
            const unmarkedSum = getUnmarkedSum(boardIndex)
            console.log("BINGO " + boardIndex + " - " + unmarkedSum*bingoNumber)
            //throw new Error("found it")
        }
    }
}
