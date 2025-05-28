-- change gamestates and initialize scenes
-- global variables


--[[
todo:
----------------
pause screen
setting screen
add score counter
add win text, timer
----------------
add sound effects
add music
add custom shaders              <-- cool
-----------------
add multiplayer                 <--- hard

]]
gc = love.graphics
gamestate = ""

menu = require "scenes.menufuncs"
game = require "scenes.gamefuncs"
settings = require "scenes.settingsfunc"
intro = require "scenes.intro"
pause = require "scenes.Pause"

windowWidth = 800
windowHeight = 500

local DEBUG = true
-- run on app start
function love.load()
    love.window.setTitle("Pong")
    love.window.setMode(windowWidth, windowHeight)
    gamestate = "intro"
end

-- draws every scene
function love.draw()
    local drawFunctions= {
        intro = function ()
            intro.draw()
        end,
        menu = function ()
            menu.draw()
        end,
        singlePlayer = function ()
            game.draw()         --draws everything in the game
            game.singlePlayer() --updates the game mechanics    
        end,
        pause = function ()
            pause.draw()
        end,
    ------------------<todo>-----------------
        settings = function ()
            
        end,
        multiplayer = function ()
        
        end,
    }
    ------------------<todo>-----------------   

    if drawFunctions[gamestate] then
            drawFunctions[gamestate]()
    end
    if debug then
        gc.print("gamestate: "..gamestate, 10, 10)
        gc.print("gamestate: "..gamestate, 10, 20)
    end
end

function love.keypressed(key)
    --table of gamestates and their action subtables
    local stateActions = { 
        menu = {
            escape = love.event.quit,
            ["1"] = function() gamestate = "singlePlayer" end,
            ["2"] = function() gamestate = "multiplayer" end,
            ["3"] = function() gamestate = "settings" end,
        },
        intro = {
            escape = love.event.quit,
            any = function() gamestate = "menu" end,
        },
        singlePlayer = {
            escape = function() gamestate = "pause" end,
        },
        pause = {
            escape = function() gamestate = "singlePlayer" end,
        }
    }
    -- Check if the current gamestate has a corresponding action table
    local actions = stateActions[gamestate] --
    if actions then             --if the gamestate has a corresponding action table
        if actions[key] then    --if the key pressed is in the action table
            actions[key]()      --call the function associated with the key
        elseif actions.any then --if the action table has an "any" key
            actions.any()
        end
    end
end
