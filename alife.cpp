#include <cmath>
#include <sstream>
#include <iostream>
#include <fstream>

#ifdef _WIN32
#define NOMINMAX
#include <Windows.h>
#endif

#define ALIFE_API __declspec(dllexport)

float smoothDamp(float current, float target, float& currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
{
    smoothTime = std::max(0.0001f, smoothTime);
    float omega = 2.0f / smoothTime;

    float x = omega * deltaTime;
    float exp = 1.0f / (1.0f + x + 0.48f * x * x + 0.235f * x * x * x);

    float change = current - target;
    float originalTo = target;

    float maxChange = maxSpeed * smoothTime;
    change = std::max(-maxChange, std::min(maxChange, change));

    target = current - change;

    float temp = (currentVelocity + omega * change) * deltaTime;
    currentVelocity = (currentVelocity - omega * temp) * exp;
    float output = target + (change + temp) * exp;

    if (originalTo - current > 0.0f == output > originalTo)
    {
        output = originalTo;
        currentVelocity = (output - originalTo) / deltaTime;
    }

    return output;
}

extern "C" {
    ALIFE_API void* createAlife(float x, float y);
    ALIFE_API void updateAlife(void* alife, float foodX, float foodY, float deltaTime);
    ALIFE_API void destroyAlife(void* alife);
    ALIFE_API void debugLog(const char* message);
}

class Alife {
public:
    Alife(float x, float y) : x(x), y(y), vx(0.0f), vy(0.0f), targetX(x), targetY(y), radius(1.0f), life(10000) {
        std::stringstream ss;
        ss << "Alife created at (" << x << ", " << y << ")";
        debugLog(ss.str().c_str());
    }

    ~Alife() {
        debugLog("Alife destroyed");
    }
    
    void init() {
        // 初期化処理
    }

    void update(float foodX, float foodY, float deltaTime) {
        // 目標位置を餌の位置に設定（徐々に追いかける）
        targetX = foodX;
        targetY = foodY;
        
        // 計算の安全対策：極端に大きなdeltaTimeを制限
        float clampedDeltaTime = std::min(deltaTime, 0.1f);
        
        // 現在位置と目標位置の差を計算
        float dx = targetX - x;
        float dy = targetY - y;
        float dist = std::sqrt(dx * dx + dy * dy);
        
        if (dist > 0.001f) { // 十分に近くない場合
            // 方向ベクトルを計算
            float dirX = dx / dist;
            float dirY = dy / dist;
            
            // 穏やかな速度で移動（前回の問題を修正）
            float moveSpeed = 0.5f;  // 穏やかな速度
            
            // 単純な線形移動（振動を防ぐため）
            float step = moveSpeed * clampedDeltaTime;
            
            // 距離よりも大きく移動しないようにする
            step = std::min(step, dist);
            
            // 位置を更新（オーバーシュートを防ぐ）
            x += dirX * step;
            y += dirY * step;
            
            // 速度を記録（表示用）
            vx = dirX * moveSpeed;
            vy = dirY * moveSpeed;
        }

        // 仮のFoodRadius
        float foodRadius = 0.5f;

        // 餌を食べる
        if (eatFood(foodX, foodY, foodRadius)) {
            // 餌を食べた処理
        }

        life--;
        std::stringstream ss;
        ss << "Alife updated: x=" << x << ", y=" << y << ", targetX=" << targetX << ", targetY=" << targetY
           << ", radius=" << radius << ", life=" << life 
           << ", vx=" << vx << ", vy=" << vy << ", deltaTime=" << deltaTime
           << ", dist=" << dist;
        debugLog(ss.str().c_str());
    }

    bool isDead() const {
        return life <= 0;
    }

    bool eatFood(float foodX, float foodY, float foodRadius) {
        float dx = foodX - x;
        float dy = foodY - y;
        float dist = std::sqrt(dx * dx + dy * dy);

        if (dist < radius + foodRadius) {
            radius += foodRadius * 0.1f / 100000.0f; // 要望通り現状維持
            life += 10;
            std::stringstream ss;
            ss << "Alife ate food at (" << foodX << ", " << foodY << ")";
            debugLog(ss.str().c_str());
            return true;
        }
        return false;
    }

public:
    float x;
    float y;
    float vx;
    float vy;
    float radius;
private:
    int life;
    float targetX;  // 目標とするX座標
    float targetY;  // 目標とするY座標
};

extern "C" {
    ALIFE_API void* createAlife(float x, float y) {
        debugLog("createAlife called");
        Alife* alife = new (std::nothrow) Alife(x, y);
        if (alife == nullptr) {
            debugLog("Failed to allocate memory for Alife");
            return nullptr;
        }
        alife->init();
        return alife;
    }

    ALIFE_API void updateAlife(void* alife, float foodX, float foodY, float deltaTime) {
        if (alife != nullptr) {
            static_cast<Alife*>(alife)->update(foodX, foodY, deltaTime);
        }
    }

    ALIFE_API void destroyAlife(void* alife) {
        debugLog("destroyAlife called");
        delete static_cast<Alife*>(alife);
    }

    ALIFE_API float getAlifeX(void* alife) {
        return static_cast<Alife*>(alife)->x;
    }

    ALIFE_API float getAlifeY(void* alife) {
        return static_cast<Alife*>(alife)->y;
    }

    ALIFE_API float getAlifeRadius(void* alife) {
        return static_cast<Alife*>(alife)->radius;
    }

    ALIFE_API float getAlifeVx(void* alife) {
        return static_cast<Alife*>(alife)->vx;
    }

    ALIFE_API float getAlifeVy(void* alife) {
        return static_cast<Alife*>(alife)->vy;
    }

    ALIFE_API void debugLog(const char* message) {
        std::ofstream logFile("G:/VSCode_src/c++/alife/ALife_log.txt", std::ios::app);
        if (logFile.is_open()) {
            logFile << "debugLog called: " << message << std::endl;
            logFile.close();
        } else {
            std::cerr << "Failed to open log file: G:/VSCode_src/c++/alife/ALife_log.txt" << std::endl;
        }
    }

    ALIFE_API bool isDead(void* alife) {
        return static_cast<Alife*>(alife)->isDead();
    }

    ALIFE_API float getFoodRadius() {
        return 0.5f;
    }
}
