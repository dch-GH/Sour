#version 440 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aTexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec4 aObjectId;

out vec3 Normal;
out vec3 FragPos;
out vec3 UV;
out vec4 ObjectId;

void main()
{
    gl_Position = projection * view * model * vec4(aPos, 1.0);
    FragPos = vec3(model * vec4(aPos, 1.0));
    Normal = aNormal;
    UV = aTexCoords;
    ObjectId = aObjectId;
}
