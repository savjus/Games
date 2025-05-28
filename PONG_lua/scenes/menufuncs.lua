

function draw()
    gc.rectangle("line", 50, 85, 200, 50)
    gc.print("single player", 100, 100)

    gc.rectangle("line", 50, 135, 200, 50)
    gc.print("multiplayer", 100, 150)
    
    gc.rectangle("line", 50, 185, 200, 50)
    gc.print("settings", 100, 200)
end

function love.mousepressed(x,y,button,istouch)
    --buttons
    if button == 1 and gamestate == "menu" then
        if x > 50 and x < 250 and y > 85 and y < 135 then
            gamestate = "singlePlayer"
        end
        if x > 50 and x < 250 and y > 135 and y < 185 then
            gamestate = "multiplayer"
        end
        if x > 50 and x < 250 and y > 185 and y < 235 then
            gamestate = "settings"
        end
    end
    
end

return {
    draw = draw
}