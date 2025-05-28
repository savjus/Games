local Ball = {
 x = 400,
 y = 300,
 radius = 10,
 xSpeed = -7,
 ySpeed = 4,
 immune = 0
}

function reset(dir)
    Ball.x = 400
    Ball.y = 300
    Ball.xSpeed = (dir == 0 and 1 or -dir / math.abs(dir)) * 7
    Ball.ySpeed = math.random(-5,5)
end

function draw()
    gc.circle("fill",Ball.x, Ball.y, Ball.radius)
end


function move(xSpeed, ySpeed)
    Ball.x = Ball.x + Ball.xSpeed
    Ball.y = Ball.y + Ball.ySpeed
    if Ball.immune > 0 then
        Ball.immune = Ball.immune - 0.1
    end
end

function BounceOffPaddle(paddle)
    local paddleCenterY = paddle.y + paddle.height / 2
    local distanceFromCenter = Ball.y - paddleCenterY
    local normalizedDistance = distanceFromCenter / (paddle.height)
    local bounceAngle = normalizedDistance * math.pi / 3

    Ball.immune = 2
    Ball.xSpeed = -Ball.xSpeed
    Ball.ySpeed = Ball.ySpeed - math.sin(bounceAngle) * Ball.xSpeed
end

return {
    reset = reset,
    Ball = Ball,
    draw = draw,
    move = move,
    BounceOffPaddle = BounceOffPaddle
}