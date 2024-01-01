#version 440 core

layout (location = 0) in vec3 aPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform float time;

flat out vec3 startPos;
out vec3 vertPos;

void main()
{
    gl_Position = projection * view * model * vec4(aPos, 1.0);
    vertPos = gl_Position.xyz / gl_Position.w;
    startPos = vertPos;
}
