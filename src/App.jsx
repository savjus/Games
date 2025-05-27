import React from "react";
//npm install react-router-dom
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import TicTacToe from "./components/TicTacToe";
import "./index.css";

function App() {
  return (
    <Router>
      <div className="app">
        {/* Navigation Bar */}
        <nav className="navbar">
          <ul className="nav-links">
            <li><Link to="/tictactoe">Tic Tac Toe</Link></li>
            <li><Link to="/chess">Chess</Link></li>
            <li><Link to="/sudoku">Minesweeper</Link></li>
          </ul>
        </nav>

        {/* Main Content */}
        <div className="content">
          <Routes>
            <Route path="/" element={<h1>Welcome to the Game Hub</h1>} />
            <Route path="/tictactoe" element={<TicTacToe />} />
            <Route path="/chess" element={<h1>Chess Coming Soon!</h1>} />
            <Route path="/sudoku" element={<h1>Minesweeper Coming Soon!</h1>} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;