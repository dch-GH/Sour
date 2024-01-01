#version 440 core

layout (location = 0) in vec3 aPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform float time;

void main()
{
    if(gl_VertexID % 2 == 0)
    {
        gl_Position = projection * view * model * vec4(0.0f, 0.0f, 0.0f, 1.0f);
    }
    else
    {

        gl_Position = projection * view * model * vec4(aPos, 1.0);
    }
}
