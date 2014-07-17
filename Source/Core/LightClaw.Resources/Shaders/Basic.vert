#version 400

uniform mat4 worldMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

in vec3 inVertexPosition;
in vec3 inVertexColor;

out vec3 passVertexColor;

void main(void)
{
	mat4 worldViewProjectionMatrix = worldMatrix * viewMatrix * projectionMatrix;
	gl_Position = vec4(inVertexPosition, 1.0) * worldViewProjectionMatrix;
	passVertexColor = inVertexColor;
}