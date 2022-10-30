#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace1;
    vec4 FragPosLightSpace2;
} fs_in;

uniform sampler2D diffuseTexture;
uniform sampler2D shadowMap1;

uniform vec3 lightPos1;
uniform vec3 viewPos;

float ShadowCalculation(vec4 fragPosLightSpace1,vec4 fragPosLightSpace2)
{
    // 已经在标准视见体中？//转为三维时要注意w的值w为1才有意义
    vec3 projCoords1 = fragPosLightSpace1.xyz / fragPosLightSpace1.w;
    vec3 projCoords2 = fragPosLightSpace2.xyz / fragPosLightSpace2.w;
    // transform to [0,1] range
    projCoords1 = projCoords1 * 0.5 + 0.5;
    projCoords2 = projCoords2 * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth1 = texture(shadowMap1, projCoords1.xy).r; 
    //float closestDepth2 = texture(shadowMap, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // 如果当前值大于深度值则是阴影 ps 深度值越远越大
    float shadow = currentDepth > closestDepth  ? 1.0 : 0.0;

    return shadow;
}

void main()
{           
    vec3 color = texture(diffuseTexture, fs_in.TexCoords).rgb;
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightColor = vec3(0.3);
    // ambient
    vec3 ambient = 0.3 * lightColor;
    // diffuse
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;    
    // calculate shadow
    float shadow = ShadowCalculation(fs_in.FragPosLightSpace1,fs_in.FragPosLightSpace2);                      
    vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;    
    
    FragColor = vec4(lighting, 1.0);
}