-- singlePlayer gamemode

local LPaddle ={
    x = 100,
    y = 100,
    width = 30,
    height = 100,
    speed = 10
    }
local RPaddle={
    x = 700,
    y = 100,
    width = 30,
    height = 100,
    speed = 10
}

local Ball = require "Ball"


local function checkInput()
    if love.keyboard.isDown("w") and RPaddle.y  > 0 then
        RPaddle.y = RPaddle.y - RPaddle.speed
    end

    if love.keyboard.isDown("s") and RPaddle.y < windowHeight-100 then
        RPaddle.y = RPaddle.y + RPaddle.speed
    end

    if love.keyboard.isDown("d")then
        Ball.reset(Ball.Ball.xSpeed)
    end
    if love.keyboard.isDown("a") then
        Ball.Ball.xSpeed = Ball.Ball.xSpeed * 1.1
    end
end


local function paddleContact(balll,ballr)
    return 
    (ballr >= RPaddle.x and ballr < RPaddle.x + RPaddle.width and 
    Ball.Ball.y > RPaddle.y and Ball.Ball.y < RPaddle.y + RPaddle.height and Ball.Ball.immune < 0.1 )
    or
    (balll <= LPaddle.x + LPaddle.width and balll > LPaddle.x and
    Ball.Ball.y > LPaddle.y and Ball.Ball.y < LPaddle.y + LPaddle.height  and Ball.Ball.immune < 0.1 )
end

local function CalculatePredictedY(ball)
    local distToPad = LPaddle.x - ball.x
    local timeToPad = distToPad / ball.xSpeed
    local predictedY = ball.y + ball.ySpeed * timeToPad

    -- Calculate the number of bounces
    local bounces = math.floor(predictedY / windowHeight)
    if bounces % 2 == 0 then
        predictedY = predictedY % windowHeight
    else
        predictedY = windowHeight - (predictedY % windowHeight)
    end

    return predictedY
end

local function AImove(ball)
    if ball.xSpeed < 0 then
        local predictedY = CalculatePredictedY(ball)

        -- Move up
        if predictedY < LPaddle.y + LPaddle.height / 2 then
            LPaddle.y = LPaddle.y - LPaddle.speed
        end
        -- Move down
        if predictedY > LPaddle.y + LPaddle.height / 2 then
            LPaddle.y = LPaddle.y + LPaddle.speed
        end

        -- Ensure the paddle stays within the window boundaries
        if LPaddle.y < 0 then
            LPaddle.y = 0
        elseif LPaddle.y + LPaddle.height > windowHeight then
            LPaddle.y = windowHeight - LPaddle.height
        end
    end
end

function draw()
    if gamestate == "singlePlayer" then
        -- draw the paddles
        gc.rectangle("fill", RPaddle.x, RPaddle.y, RPaddle.width, RPaddle.height, 10, 10)
        gc.rectangle("fill", LPaddle.x, LPaddle.y, LPaddle.width, LPaddle.height, 10, 10)
        -- draw the ball
        Ball.draw()
    end
end

local function singlePlayer()
    local ballr = Ball.Ball.x + Ball.Ball.radius
    local balll = Ball.Ball.x - Ball.Ball.radius
    checkInput()
    Ball.move(Ball.Ball.xSpeed, Ball.Ball.ySpeed)
    AImove(Ball.Ball)
    --bounce ball off top and bottom
    if Ball.Ball.y < 0 or Ball.Ball.y > windowHeight then
        Ball.Ball.ySpeed = -Ball.Ball.ySpeed
    end
    -- reset ball if it goes off screen
    if Ball.Ball.x < 0 or Ball.Ball.x > windowWidth then
        Ball.reset(Ball.Ball.xSpeed)
    end
    --bounce ball off paddles add immunity 
    if paddleContact(balll,ballr)then

        if Ball.Ball.xSpeed < 0 then
            Ball.BounceOffPaddle(LPaddle)
        else
            Ball.BounceOffPaddle(RPaddle)
        end
    end

end

return {
    LPaddle = LPaddle,
    RPaddle = RPaddle,
    draw = draw,
    checkInput = checkInput,
    singlePlayer = singlePlayer
}
