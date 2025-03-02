#include <cmath>
#include <sstream>
#include <iostream>

#ifdef _WIN32
#include <Windows.h>
#endif

//#using <mscorlib.dll>
//#using <UnityEngine.dll>

#define ALIFE_API __declspec(dllexport)

//using namespace System;

extern "C" {
    ALIFE_API void* createAlife(float x, float y);
    ALIFE_API void updateAlife(void* alife, float foodX, float foodY);
    ALIFE_API void destroyAlife(void* alife);
    ALIFE_API void debugLog(const char* message);
}

class Alife {
public:
    Alife(float x, float y) : x(x), y(y), vx(0.01f), vy(0.02f), radius(1.0f), life(100) {
        std::stringstream ss;
        ss << "Alife created at (" << x << ", " << y << ")";
        debugLog(ss.str().c_str());
    }

    ~Alife() {
        debugLog("Alife destroyed");
    }

    void update(float foodX, float foodY) {
        // Move towards food
        float dx = foodX - x;
        float dy = foodY - y;
        float dist = std::sqrt(dx * dx + dy * dy);

        if (dist > 0.0f) {
            dx /= dist;
            dy /= dist;
        }

        x += dx * vx;
        y += dy * vy;

        // Check if food is eaten
        if (dist < radius) {
            radius += 0.00001f;
            life += 10; // Increase life more when eating food
            std::stringstream ss;
            ss << "Alife ate food. Radius: " << radius << ", Life: " << life;
            debugLog(ss.str().c_str());
        }

        life--;
        std::stringstream ss;
        ss << "Alife updated: x=" << x << ", y=" << y <<  ", radius=" << radius << ", life=" << life;
        debugLog(ss.str().c_str());
    }

    bool isDead() const {
        return life <= 0;
    }
public:
    float x;
    float y;
    float vx;
    float vy;
    float radius;
private:
    int life;
};

extern "C" {
    ALIFE_API void* createAlife(float x, float y) {
        return new Alife(x, y);
    }

    ALIFE_API void updateAlife(void* alife, float foodX, float foodY) {
        static_cast<Alife*>(alife)->update(foodX, foodY);
    }

    ALIFE_API void destroyAlife(void* alife) {
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

     ALIFE_API void debugLog(const char* message) {
        OutputDebugStringA(message);
    }

    ALIFE_API bool isDead(void* alife) {
        return static_cast<Alife*>(alife)->isDead();
    }
}
